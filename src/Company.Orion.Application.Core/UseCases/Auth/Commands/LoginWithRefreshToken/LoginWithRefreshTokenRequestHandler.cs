using MediatR;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Crosscutting.Resources;
using Microsoft.Extensions.Localization;
using Company.Orion.Domain.Core.Exceptions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Extensions;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenRequestHandler(IUnitOfWork unitOfWork, IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<LoginWithRefreshTokenRequest, LoginWithRefreshTokenResponse>
{
    public async Task<LoginWithRefreshTokenResponse> Handle(LoginWithRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw CreateInvalidRefreshTokenException();
        }

        var email = GetClaimFromJwtToken(request.Token, ClaimTypes.Email);

        var currentRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(request.RefreshToken) && x.Email == email)).FirstOrDefault()
            ?? throw CreateInvalidRefreshTokenException();

        var user = (await unitOfWork.UserRepository.SearchByAsync(x => x.Email == currentRefreshToken.Email)).FirstOrDefault()
            ?? throw CreateInvalidRefreshTokenException();

        var newRefreshToken = await RotateRefreshTokenAsync(currentRefreshToken);

        return new LoginWithRefreshTokenResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile,
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
    private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken currentRefreshToken)
    {
        var newRefreshToken = new RefreshToken
        {
            Email = currentRefreshToken.Email,
            Refreshtoken = Guid.NewGuid().ToString().ToSha512()
        };

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await unitOfWork.RefreshTokenRepository.DeleteAsync(currentRefreshToken.PublicId);

                newRefreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);

                await unitOfWork.CommitAsync();
            });
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

    private string GetClaimFromJwtToken(string jtwToken, string claimName)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jtwToken);
            var claimValue = jwtSecurityToken.Claims.First(claim => claim.Type == claimName).Value;

            return claimValue;
        }
        catch (Exception)
        {
            throw CreateInvalidRefreshTokenException();
        }
    }

    private UnauthorizedUserException CreateInvalidRefreshTokenException()
    {
        return new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
    }
}
