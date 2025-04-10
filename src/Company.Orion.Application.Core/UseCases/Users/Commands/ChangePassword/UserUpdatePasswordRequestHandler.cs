using Company.Orion.Application.Core.UseCases.Users.Notifications.UserPasswordChanged;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword;

public class UserUpdatePasswordRequestHandler(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMediator mediator,
    IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<UserChangePasswordRequest>
{
    public async Task Handle(UserChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(currentUser.Id) ?? throw new NotFoundException(currentUser.Id);

        if (user.Password != request.CurrentPassword.ToSha512())
            throw new BusinessException(resourceMessages[UserMessages.InvalidPassword]);

        user.Password = request.NewPassword.ToSha512();

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new UserPasswordChangedNotification(), cancellationToken);
    }
}
