using Company.Orion.Application.Core.UseCases.Users.Notifications.UserPasswordChanged;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Authentication;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword;

public class UserUpdatePasswordRequestHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser,
    IMediator mediator,
    IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<UserChangePasswordRequest>
{
    public async Task Handle(UserChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(currentUser.Id, cancellationToken) ?? throw new NotFoundException(currentUser.Id);

        if (passwordHasher.Verify(user.Password, request.CurrentPassword) == PasswordVerificationResult.Failed)
            throw new BusinessException(resourceMessages[UserMessages.InvalidPassword]);

        user.Password = passwordHasher.Hash(request.NewPassword);

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);

        await mediator.Publish(new UserPasswordChangedNotification(), cancellationToken);
    }
}
