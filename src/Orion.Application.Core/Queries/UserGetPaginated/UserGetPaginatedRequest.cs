using MediatR;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;

namespace Orion.Application.Core.Queries.UserGetPaginated;

public class UserGetPaginatedRequest :  UserFilter, IRequest<PagedList<User>>
{
    
}