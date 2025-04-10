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
            throw new UnauthorizedUserException(
                resourceMessages[UserMessages.InvalidRefreshToken],
                resourceMessages[ExceptionsTitles.AuthenticationError]
            );
        }

        var email = GetClaimFromJwtToken(request.Token, ClaimTypes.Email);

        var userRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Refreshtoken.Equals(request.RefreshToken) && x.Email == email)).FirstOrDefault();

        if (userRefreshToken is not null)
        {
            var user = (await unitOfWork.UserRepository.SearchByAsync(x => x.Email == userRefreshToken.Email)).FirstOrDefault();

            if (user is not null)
            {
                await unitOfWork.RefreshTokenRepository.DeleteAsync(userRefreshToken.PublicId);
                var newRefreshToken = await AddRefreshTokenAsync(user.Email);

                return new LoginWithRefreshTokenResponse
                {
                    Email = user.Email,
                    Name = user.Name,
                    PublicId = user.PublicId,
                    Profile = user.Profile,
                    RefreshToken = newRefreshToken.Refreshtoken
                };
            }

        }

        throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
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
            throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidRefreshToken], resourceMessages[ExceptionsTitles.AuthenticationError]);
        }
    }

    private async Task<RefreshToken> AddRefreshTokenAsync(string userEmail)
    {
        var existentRefreshToken = (await unitOfWork.RefreshTokenRepository.SearchByAsync(x => x.Email == userEmail)).FirstOrDefault();

        if (existentRefreshToken is not null)
            return existentRefreshToken;

        var refreshToken = new RefreshToken
        {
            Email = userEmail,
            Refreshtoken = Guid.NewGuid().ToString().ToSha512()
        };

        var addedRefreshToken = await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await unitOfWork.CommitAsync();

        return addedRefreshToken;
    }
}