using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdHandler : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    private readonly IUserService _userService;
    
    public UserGetByIdHandler(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
    {
        return (UserGetByIdResponse) await _userService.FindByIdAsync(request.PublicId);
    }
}