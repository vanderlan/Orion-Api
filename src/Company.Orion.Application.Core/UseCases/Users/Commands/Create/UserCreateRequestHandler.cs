using Company.Orion.Application.Core.UseCases.Users.Notifications.UserCreated;
using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Entities;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Create;

public class UserCreateRequestHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IStringLocalizer<OrionResources> resourceMessages) : IRequestHandler<UserCreateRequest, UserCreateResponse>
{
    public async Task<UserCreateResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
    {
        await ValidateUser(request);

        request.Password = request.Password.ToSha512();

        var added = await unitOfWork.UserRepository.AddAsync((User)request);
        await unitOfWork.CommitAsync();


        await mediator.Publish(new UserCreatedNotification(added), cancellationToken);

        return (UserCreateResponse)added;
    }

    private async Task ValidateUser(UserCreateRequest request)
    {
        var userFound = await unitOfWork.UserRepository.FindByEmailAsync(request.Email);

        if (userFound != null)
            throw new ConflictException(resourceMessages[UserMessages.EmailExists], resourceMessages[ExceptionsTitles.ValidationError]);
    }
}