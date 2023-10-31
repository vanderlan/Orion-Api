using MediatR;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Extensions;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.LoginWithCredentials;

public class LoginWithCredentialsRequestHandler : IRequestHandler<LoginWithCredentialsRequest, LoginWithCredentialsResponse>
{
    private readonly IUserService _userService;
    
    public LoginWithCredentialsRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<LoginWithCredentialsResponse> Handle(LoginWithCredentialsRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.LoginAsync(request.Email, request.Password);

        var refreshToken = await _userService.AddRefreshTokenAsync(
          new RefreshToken
          {
              Email = user.Email,
              Refreshtoken = Guid.NewGuid().ToString().ToSha512()
          });

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