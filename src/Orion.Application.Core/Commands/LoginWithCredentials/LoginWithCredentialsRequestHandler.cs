using MediatR;
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

        return (LoginWithCredentialsResponse)user;
    }
}