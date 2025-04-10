using MediatR;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Crosscutting.Resources;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;

public class LoginWithCredentialsRequestHandler(IUnitOfWork unitOfWork, IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<LoginWithCredentialsRequest, LoginWithCredentialsResponse>
{
    public async Task<LoginWithCredentialsResponse> Handle(LoginWithCredentialsRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.LoginAsync(request.Email, request.Password.ToSha512());

        if (user is not null)
        {
            var refreshToken = await AddRefreshTokenAsync(user.Email);

            return new LoginWithCredentialsResponse
            {
                Email = user.Email,
                Name = user.Name,
                PublicId = user.PublicId,
                Profile = user.Profile,
                RefreshToken = refreshToken.Refreshtoken
            };
        }

        throw new UnauthorizedUserException(resourceMessages[UserMessages.InvalidCredentials], resourceMessages[ExceptionsTitles.AuthenticationError]);
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