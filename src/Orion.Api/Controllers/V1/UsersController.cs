using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Asp.Versioning;
using Orion.Api.Attributes;
using Orion.Api.AutoMapper.Input;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Controllers.Base;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;
using static Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/[controller]")]
[AuthorizeFor(Roles.Admin)]
public class UsersController : ApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, IMapper mapper) : base(mapper)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromQuery] BaseFilter<User> filter)
    {
        var user = await _userService.ListPaginateAsync(filter);

        var userOutputList = Mapper.Map<PagedList<UserOutput>>(user);

        return Ok(userOutputList);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _userService.FindByIdAsync(id);
        var userOutput = Mapper.Map<UserOutput>(user);

        if (userOutput is null)
            return NotFound();

        return Ok(userOutput);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Post([FromBody] UserInput userInput)
    {
        var user = Mapper.Map<User>(userInput);

        var created = await _userService.AddAsync(user);

        return Created(Mapper.Map<UserOutput>(created));
    }

    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> Put(string id, [FromBody] UserInput userInput)
    {
        userInput.PublicId = id;
        var user = Mapper.Map<User>(userInput);

        await _userService.UpdateAsync(user);

        return Accepted();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteAsync(id);

        return NoContent();
    }
}
