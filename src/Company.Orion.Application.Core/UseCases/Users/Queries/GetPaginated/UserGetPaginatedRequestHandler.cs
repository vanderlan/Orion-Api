using Company.Orion.Domain.Core.Repositories.UnitOfWork;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;

public class UserGetPaginatedRequestHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UserGetPaginatedRequest, PagedList<UserGetPaginatedResponse>>
{
    public async Task<PagedList<UserGetPaginatedResponse>> Handle(UserGetPaginatedRequest request, CancellationToken cancellationToken)
    {
        var users = await unitOfWork.UserRepository.ListPaginateAsync(request);

        var usersPaginated = users.Items.Select(user => (UserGetPaginatedResponse)user).ToList();

        return new PagedList<UserGetPaginatedResponse>(usersPaginated, users.Count);
    }
}