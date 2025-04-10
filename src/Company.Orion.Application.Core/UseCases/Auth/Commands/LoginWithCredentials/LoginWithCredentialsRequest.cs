using MediatR;

namespace Company.Orion.Application.Core.UseCases.Auth.Commands.LoginWithCredentials;

public class LoginWithCredentialsRequest : IRequest<LoginWithCredentialsResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
}