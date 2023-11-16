using MediatR;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Application.Core.Queries.UserGetPaginated;

public class UserGetPaginatedRequestHandler : IRequestHandler<UserGetPaginatedRequest, PagedList<UserGetPaginatedResponse>>
{
    private readonly IUserService _userService;
    
    public UserGetPaginatedRequestHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<PagedList<UserGetPaginatedResponse>> Handle(UserGetPaginatedRequest request, CancellationToken cancellationToken)
    {
        var users = await _userService.ListPaginateAsync(request);

        var usersPaginated = users.Items.Select(user => (UserGetPaginatedResponse)user).ToList();

        return new PagedList<UserGetPaginatedResponse>(usersPaginated, users.Count);
    }
}