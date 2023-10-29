using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using Orion.Api.Models;

namespace Orion.Api.Controllers.Base;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected AuthUserModel AuthUser => GetAuthenticatedUser();
    protected ApiController(IMediator mediator)
    {
        Mediator = mediator;
    }

    private AuthUserModel GetAuthenticatedUser()
    {
        var email = ((ClaimsIdentity)User.Identity)?.FindFirst(ClaimTypes.Email);
        var givenName = ((ClaimsIdentity)User.Identity)?.FindFirst(ClaimTypes.GivenName);

        return new AuthUserModel
        (
            PublicId: User?.Identity?.Name,
            Email: email?.Value,
            Name: givenName?.Value
        );
    }

    protected CreatedResult Created(object entity)
    {
        return base.Created("{id}", entity);
    }
}