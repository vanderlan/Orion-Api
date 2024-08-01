using Company.Orion.Application.Core.UseCases.Users.Notifications.UserCreated;
using Company.Orion.Domain.Core.Services.Interfaces;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Create;

public class UserCreateRequestHandler(IUserService userService, IMediator mediator) : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var userCreated = await userService.AddAsync(request);

        await mediator.Publish(new UserCreatedNotification(userCreated), cancellationToken);

        return (UserCreateResponse)userCreated;
    }
}