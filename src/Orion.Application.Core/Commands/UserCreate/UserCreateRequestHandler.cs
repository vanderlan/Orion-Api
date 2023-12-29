using MediatR;
using Orion.Application.Core.Notifications.UserCreated;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserCreate;

public class UserCreateRequestHandler(IUserService userService, IMediator mediator)
    : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var userCreated = await userService.AddAsync(request);

        await mediator.Publish(new UserCreatedNotification(userCreated), cancellationToken);

        return (UserCreateResponse)userCreated;
    }
}