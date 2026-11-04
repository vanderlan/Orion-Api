using Asp.Versioning;
using Company.Orion.Api.Attributes;
using Company.Orion.Api.Controllers.Base;
using Company.Orion.Application.Core.UseCases.Users.Commands.ChangePassword;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Application.Core.UseCases.Users.Commands.Delete;
using Company.Orion.Application.Core.UseCases.Users.Commands.Update;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetById;
using Company.Orion.Application.Core.UseCases.Users.Queries.GetPaginated;
using Company.Orion.Domain.Core.ValueObjects.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Company.Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Company.Orion.Api.Controllers.V1;

/// <summary>
/// Users management. Requires the <c>admin</c> profile.
/// </summary>
[ApiVersion(1.0)]
[Route("[controller]")]
[AuthorizeFor(Roles.Admin)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class UsersController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Lists users with pagination.
    /// </summary>
    /// <remarks>Use <c>query</c> to filter by name. <c>count</c> is the total number of records.</remarks>
    /// <response code="200">A success response with a list of Users paginated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<UserGetPaginatedResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] UserGetPaginatedRequest filter)
    {
        return Ok(await Mediator.Send(filter));
    }

    /// <summary>
    /// Gets a user by its public id.
    /// </summary>
    /// <response code="200">A success response with a single User.</response>
    /// <response code="404">No user found with the informed id.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserGetByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var user = await Mediator.Send(new UserGetByIdRequest(id));

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <response code="201">The created User.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="409">The e-mail is already in use.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] UserCreateRequest userCreateRequest)
    {
        return Created(await Mediator.Send(userCreateRequest));
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <response code="202">The update was accepted.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="404">No user found with the informed id.</response>
    /// <response code="409">The e-mail is already in use by another user.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(string id, [FromBody] UserUpdateRequest userUpdateRequest)
    {
        userUpdateRequest.PublicId = id;

        await Mediator.Send(userUpdateRequest);

        return Accepted();
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <response code="204">The user was deleted.</response>
    /// <response code="404">No user found with the informed id.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await Mediator.Send(new UserDeleteRequest(id));

        return NoContent();
    }

    /// <summary>
    /// Changes the password of the authenticated user.
    /// </summary>
    /// <response code="202">The password was changed.</response>
    /// <response code="400">The request is invalid or the current password does not match.</response>
    /// <response code="404">The authenticated user no longer exists.</response>
    [HttpPatch("Me/Password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchChangePassword([FromBody] UserChangePasswordRequest userChangePasswordRequest)
    {
        await Mediator.Send(userChangePasswordRequest);

        return Accepted();
    }
}
