using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Asp.Versioning;
using MediatR;
using Orion.Api.Attributes;
using Orion.Api.AutoMapper.Output;
using Orion.Api.Controllers.Base;
using Orion.Application.Core.Commands.CustomerCreate;
using Orion.Application.Core.Commands.CustomerDelete;
using Orion.Application.Core.Commands.CustomerUpdate;
using Orion.Application.Core.Queries.CustomerGetById;
using Orion.Domain.Core.Services.Interfaces;
using Orion.Domain.Core.Entities;
using Orion.Domain.Core.Filters;
using Orion.Domain.Core.ValueObjects.Pagination;
using static Orion.Domain.Core.Authentication.AuthorizationConfiguration;

namespace Orion.Api.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/[controller]")]
[AuthorizeFor(Roles.Admin, Roles.Customer)]
public class CustomersController : ApiController
{
    private readonly ICustomerService _customerService;
    private readonly IMediator _mediator;
    
    public CustomersController(ICustomerService customerService, IMapper mapper, IMediator mediator) : base(mapper)
    {
        _customerService = customerService;
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromQuery] BaseFilter<Customer> filter)
    {
        var customer = await _customerService.ListPaginateAsync(filter);

        var customerOutputList = Mapper.Map<PagedList<CustomerOutput>>(customer);

        return Ok(customerOutputList);
    }

    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        return Ok(await _mediator.Send(new CustomerGetByIdQuery(id)));
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Post([FromBody] CustomerCreateCommand customerCreateCommand)
    {
        return Created(await _mediator.Send(customerCreateCommand));
    }

    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> Put(string id, [FromBody] CustomerUpdateCommand customerUpdateCommand)
    {
        customerUpdateCommand.PublicId = id;
        await _mediator.Send(customerUpdateCommand);
        
        return Accepted();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new CustomerDeleteCommand(id));

        return NoContent();
    }
}
