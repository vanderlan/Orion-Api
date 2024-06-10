﻿using MediatR;
using Company.Orion.Domain.Core.Services.Interfaces;

namespace Company.Orion.Application.Core.Commands.UserDelete;

public class UserDeleteRequestHandler(IUserService userService) : IRequestHandler<UserDeleteRequest>
{
    public async Task Handle(UserDeleteRequest request, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(request.PublicId);
    }
}