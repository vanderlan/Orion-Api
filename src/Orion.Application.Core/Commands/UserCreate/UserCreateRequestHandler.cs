using MediatR;
using Orion.Application.Core.Notifications.UserCreated;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Commands.UserCreate;

public class UserCreateRequestHandler : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    private readonly IUserService _userService;
    private readonly IMediator _mediator;

    public UserCreateRequestHandler(IUserService userService, IMediator mediator)
    {
        _userService = userService;
        _mediator = mediator;
    }

    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var userCreated = await _userService.AddAsync(request);

        await _mediator.Publish(new UserCreatedNotification(userCreated), cancellationToken);

        return (UserCreateResponse)userCreated;
    }
}