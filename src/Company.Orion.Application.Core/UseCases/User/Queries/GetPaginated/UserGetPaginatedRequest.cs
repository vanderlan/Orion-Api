using MediatR;
using Company.Orion.Domain.Core.Filters;
using Company.Orion.Domain.Core.ValueObjects.Pagination;

namespace Company.Orion.Application.Core.UseCases.User.Queries.GetPaginated;

public class UserGetPaginatedRequest : UserFilter, IRequest<PagedList<UserGetPaginatedResponse>>;