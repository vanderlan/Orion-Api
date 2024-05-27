using MediatR;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.ValueObjects.Pagination;

namespace Company.Orion.Application.Core.Queries.UserGetPaginated;

public class UserGetPaginatedRequest :  UserFilter, IRequest<PagedList<UserGetPaginatedResponse>>;