using Company.Orion.Application.Core.Notifications.UserCreated;
using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.UseCases.User.Commands.Create;

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