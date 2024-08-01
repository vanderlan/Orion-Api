using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;

public class UserGetPaginatedRequest : UserFilter, IRequest<PagedList<UserGetPaginatedResponse>>;