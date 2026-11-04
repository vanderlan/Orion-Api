using Company.Orion.Application.Core.UseCases.Users.Notifications.UserCreated;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Create;

public class UserCreateRequestHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMediator mediator,
        IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        await ValidateUser(request, cancellationToken);

        request.Password = passwordHasher.Hash(request.Password);

        var added = await unitOfWork.UserRepository.AddAsync((User)request, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);


        await mediator.Publish(new UserCreatedNotification(added), cancellationToken);

        return (UserCreateResponse)added;
    }

    private async Task ValidateUser(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var userFound = await unitOfWork.UserRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (userFound != null)
            throw new ConflictException(resourceMessages[UserMessages.EmailExists], resourceMessages[ExceptionsTitles.ValidationError]);
    }
}