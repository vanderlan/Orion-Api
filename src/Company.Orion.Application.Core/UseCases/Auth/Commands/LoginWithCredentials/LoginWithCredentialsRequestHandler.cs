using System.Buffers.Text;
using System.Security.Cryptography;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;

public class LoginWithCredentialsRequestHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<LoginWithCredentialsRequest, LoginWithCredentialsResponse>
{
    public async Task<LoginWithCredentialsResponse> Handle(LoginWithCredentialsRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.FindByEmailAsync(request.Email, cancellationToken);

        //The same error is returned for unknown e-mail and wrong password (no user enumeration).
        var verification = user is null
            ? PasswordVerificationResult.Failed
            : passwordHasher.Verify(user.Password, request.Password);

        if (verification == PasswordVerificationResult.Failed)
            throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidCredentials], resourceMessages[ExceptionsTitles.AuthenticationError]);

        var refreshToken = await IssueRefreshTokenAsync(user, request.Password, verification == PasswordVerificationResult.SuccessRehashNeeded, cancellationToken);

        var accessToken = tokenService.CreateAccessToken(user);

        return new LoginWithCredentialsResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile,
            Token = accessToken.Token,
            Expiration = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Refreshtoken
        };
    }

    /// <summary>
    /// Every login revokes the refresh tokens the user still has and issues a brand new one, so a
    /// token leaked from a previous session stops working as soon as the user logs in again.
    /// When the stored password hash is outdated (legacy algorithm or lower work factor) it is
    /// upgraded in the same transaction, transparently for the user.
    /// </summary>
    private async Task<RefreshToken> IssueRefreshTokenAsync(User user, string password, bool rehashPassword, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken
        {
            Email = user.Email,
            Refreshtoken = GenerateRefreshToken()
        };

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingTokens = await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == user.Email, cancellationToken);

            foreach (var existingToken in existingTokens)
            {
                await unitOfWork.RefreshTokenRepository.DeleteAsync(existingToken.PublicId, cancellationToken);
            }

            if (rehashPassword)
            {
                user.Password = passwordHasher.Hash(password);
                unitOfWork.UserRepository.Update(user);
            }

            refreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);
        }, cancellationToken);

        return refreshToken;
    }

    /// <summary>512 bits from a cryptographic random generator, URL-safe (base64url).</summary>
    internal static string GenerateRefreshToken()
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(64));
    }
}
