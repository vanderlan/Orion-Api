using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Orion.Api.Models;

namespace Orion.Api.Controllers.Base;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly IMapper Mapper;
    protected AuthUserModel AuthUser => GetAuthenticatedUser();
    protected ApiController(IMapper mapper)
    {
        Mapper = mapper;
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    private AuthUserModel GetAuthenticatedUser()
    {
        var email = ((ClaimsIdentity)User.Identity)?.FindFirst(ClaimTypes.Email);
        var givenName = ((ClaimsIdentity)User.Identity)?.FindFirst(ClaimTypes.GivenName);

        return new AuthUserModel
        {
            PublicId = User?.Identity?.Name,
            Email = email?.Value,
            FisrtName = givenName?.Value,
        };
    }

    protected CreatedResult Created(object entity)
    {
        return base.Created("{id}", entity);
    }
}