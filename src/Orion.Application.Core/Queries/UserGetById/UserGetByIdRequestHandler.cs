using MediatR;
using Orion.Domain.Core.Services.Interfaces;

namespace Orion.Application.Core.Queries.UserGetById;

public class UserGetByIdHandler : IRequestHandler<UserGetByIdRequest, UserGetByIdResponse>
{
    private readonly IUserService _customerService;
    
    public UserGetByIdHandler(IUserService customerService)
    {
        _customerService = customerService;
    }
    
    public async Task<UserGetByIdResponse> Handle(UserGetByIdRequest command, CancellationToken cancellationToken)
    {
        return (UserGetByIdResponse) await _customerService.FindByIdAsync(command.PublicId);
    }
}