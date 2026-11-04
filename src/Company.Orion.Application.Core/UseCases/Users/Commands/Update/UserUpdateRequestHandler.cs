using Company.Orion.Crosscutting.Resources;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Localization;
using static Company.Orion.Crosscutting.Resources.Messages.MessagesKeys;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Update;

public class UserUpdateRequestHandler(IUnitOfWork unitOfWork, IStringLocalizer<OrionResources> resourceMessages) 
    : IRequestHandler<UserUpdateRequest>
{
    public async Task Handle(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(request.PublicId, cancellationToken) ?? throw new NotFoundException(request.PublicId);
        await ValidateUser(request, cancellationToken);

        user.Email = request.Email;
        user.Name = request.Name;
        user.Profile = request.Profile;

        unitOfWork.UserRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task ValidateUser(UserUpdateRequest request, CancellationToken cancellationToken)
    {
        var userFound = await unitOfWork.UserRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (userFound != null && userFound.PublicId != request.PublicId)
            throw new ConflictException(resourceMessages[UserMessages.EmailExists], resourceMessages[ExceptionsTitles.ValidationError]);
    }
}