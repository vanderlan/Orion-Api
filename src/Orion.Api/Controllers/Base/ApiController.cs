using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Orion.Api.Controllers.Base;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected ApiController(IMediator mediator)
    {
        Mediator = mediator;
    }

    protected CreatedResult Created(object entity)
    {
        return base.Created("{id}", entity);
    }
}