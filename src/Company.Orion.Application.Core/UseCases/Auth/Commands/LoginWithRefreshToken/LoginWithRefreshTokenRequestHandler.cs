using System.Security.Claims;
using Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequestHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider,
    IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<LoginWithRefreshTokenRequest, LoginWithRefreshTokenResponse>
{
    public async Task<LoginWithRefreshTokenResponse> Handle(LoginWithRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            throw CreateInvalidRefreshTokenException();

        //The expired access token must still be one issued by this API (signature, issuer and audience are
        //validated); otherwise anyone could pair a forged e-mail with a stolen refresh token.
        var principal = await tokenService.ValidateExpiredAccessTokenAsync(request.Token)
            ?? throw CreateInvalidRefreshTokenException();

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
            ?? throw CreateInvalidRefreshTokenException();

        var currentRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(request.RefreshToken) && x.Email == email, cancellationToken)).FirstOrDefault()
            ?? throw CreateInvalidRefreshTokenException();

        if (currentRefreshToken.IsExpired(timeProvider.GetUtcNow().UtcDateTime, jwtOptions.Value.RefreshTokenLifetime))
            throw CreateInvalidRefreshTokenException();

        var user = (await unitOfWork.UserRepository.SearchByAsync(x => x.Email == currentRefreshToken.Email, cancellationToken)).FirstOrDefault()
            ?? throw CreateInvalidRefreshTokenException();

        var newRefreshToken = await RotateRefreshTokenAsync(currentRefreshToken, cancellationToken);

        var accessToken = tokenService.CreateAccessToken(user);

        return new LoginWithRefreshTokenResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile,
            Token = accessToken.Token,
            Expiration = accessToken.ExpiresAt,
            RefreshToken = newRefreshToken.Refreshtoken
        };
    }

    /// <summary>
    /// Rotates the refresh token atomically: the current token is revoked and a brand new one
    /// is issued inside a single transaction, so the old token can never be served again after
    /// the rotation and the new one only becomes valid when the old one is effectively revoked.
    /// When the rotation cannot be persisted, everything is rolled back and the current token
    /// remains valid, allowing the user to retry.
    /// </summary>
    private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken currentRefreshToken, CancellationToken cancellationToken)
    {
        var newRefreshToken = new RefreshToken
        {
            Email = currentRefreshToken.Email,
            Refreshtoken = LoginWithCredentialsRequestHandler.GenerateRefreshToken()
        };

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await unitOfWork.RefreshTokenRepository.DeleteAsync(currentRefreshToken.PublicId, cancellationToken);

                newRefreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);
            }, cancellationToken);
        }
        catch (Exception exception)
        {
            throw new RefreshTokenRotationException(
                resourceMessages[UserMessages.RefreshTokenRotationFailed],
                resourceMessages[ExceptionsTitles.AuthenticationError],
                exception
            );
        }

        return newRefreshToken;
    }

    private UnauthorizedUserException CreateInvalidRefreshTokenException()
    {
        return new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
    }
}
