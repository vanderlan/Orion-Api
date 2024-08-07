using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Company.Orion.Domain.Core.Exceptions;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Company.Orion.Api.Attributes;
using Company.Orion.Api.Controllers.Base;
using Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Application.Core.UseCases.Users.Commands.Delete;
using Company.Orion.Application.Core.UseCases.Users.Commands.Update;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetById;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("[controller]")]
[AuthorizeFor(Roles.Admin)]
public class UsersController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success response with a list of Users paginated", typeof(PagedList<UserGetPaginatedResponse>))]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] UserGetPaginatedRequest filter)
    {
        return Ok(await Mediator.Send(filter));
    }

    [HttpGet("{id}")]
    [SwaggerResponse((int)HttpStatusCode.OK,"A success response with a single User", typeof(UserGetByIdResponse))]
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

    [HttpPatch("Me/Password")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "A error response with the error description", typeof(ExceptionResponse))]
    [SwaggerResponse((int)HttpStatusCode.Accepted)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> PatchChangePassword([FromBody] UserChangePasswordRequest userChangePasswordRequest)
    {
        await Mediator.Send(userChangePasswordRequest);

        return Accepted();
    }
}
