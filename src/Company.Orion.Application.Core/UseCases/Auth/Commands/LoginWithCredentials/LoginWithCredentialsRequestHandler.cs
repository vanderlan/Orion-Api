using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;

public class LoginWithCredentialsRequestHandler(IUserService userService) : IRequestHandler<LoginWithCredentialsRequest, LoginWithCredentialsResponse>
{
    public async Task<LoginWithCredentialsResponse> Handle(LoginWithCredentialsRequest request, CancellationToken cancellationToken)
    {
        var (user, refreshToken) = await userService.SignInWithCredentialsAsync(request.Email, request.Password);

        return new LoginWithCredentialsResponse
        {
            Email = user.Email,
            Name = user.Name,
            PublicId = user.PublicId,
            Profile = user.Profile,
            RefreshToken = refreshToken.Refreshtoken
        };
    }
}