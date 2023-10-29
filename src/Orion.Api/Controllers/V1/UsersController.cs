using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orion.Api.Attributes;
using Orion.Api.Controllers.Base;
using Orion.Application.Core.Commands.UserCreate;
using Orion.Application.Core.Commands.UserDelete;
using Orion.Application.Core.Commands.UserUpdate;
using Orion.Application.Core.Queries.UserGetById;
using Orion.Application.Core.Queries.UserGetPaginated;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Exceptions;
using Orion.Domain.Core.ValueObjects.Pagination;
using Swashbuckle.AspNetCore.Annotations;
using static Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/[controller]")]
[AuthorizeFor(Roles.Admin)]
public class UsersController : ApiController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
        
    }

    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success reponse with a list of Users paginated" ,typeof(PagedList<User>))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] UserGetPaginatedRequest filter)
    {
        var userOutputList = await Mediator.Send(filter);

        return Ok(userOutputList);
    }

    [HttpGet("{id}")]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success reponse with a single User" ,typeof(UserGetByIdResponse))]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Get(string id)
    {
        var user = await Mediator.Send(new UserGetByIdRequest(id));
        
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [SwaggerResponse((int)HttpStatusCode.Created,"A valid User created", typeof(UserCreateResponse))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,"A error response with the error description", typeof(ExceptionResponse))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Post([FromBody] UserCreateRequest userCreateRequest)
    {
        return Created(await Mediator.Send(userCreateRequest));
    }

    [HttpPut("{id}")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest,"A error response with the error description", typeof(ExceptionResponse))]
    [SwaggerResponse((int)HttpStatusCode.Accepted)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Put(string id, [FromBody] UserUpdateRequest userUpdateRequest)
    {
        userUpdateRequest.PublicId = id;

        await Mediator.Send(userUpdateRequest);

        return Accepted();
    }

    [HttpDelete("{id}")]
    [SwaggerResponse((int)HttpStatusCode.NoContent)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Delete(string id)
    {
        await Mediator.Send(new UserDeleteRequest(id));
        
        return NoContent();
    }
}
