using MediatR;
using Orion.Application.Core.Queries.UserGetById;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Application.Core.Queries.UserGetPaginated;

public class UserGetPaginatedRequestHandler : IRequestHandler<UserGetPaginatedRequest, PagedList<User>>
{
    private readonly IUserService _customerService;
    
    public UserGetPaginatedRequestHandler(IUserService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<PagedList<User>> Handle(UserGetPaginatedRequest request, CancellationToken cancellationToken)
    {
        return await _customerService.ListPaginateAsync(request);
    }
}