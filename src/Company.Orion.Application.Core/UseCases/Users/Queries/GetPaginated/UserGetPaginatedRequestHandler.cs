﻿using Company.Orion.Domain.Core.Services.Interfaces;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;

public class UserGetPaginatedRequestHandler(IUserService userService)
    : IRequestHandler<UserGetPaginatedRequest, PagedList<UserGetPaginatedResponse>>
{
    public async Task<PagedList<UserGetPaginatedResponse>> Handle(UserGetPaginatedRequest request, CancellationToken cancellationToken)
    {
        var users = await userService.ListPaginateAsync(request);

        var usersPaginated = users.Items.Select(user => (UserGetPaginatedResponse)user).ToList();

        return new PagedList<UserGetPaginatedResponse>(usersPaginated, users.Count);
    }
}