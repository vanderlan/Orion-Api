using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Orion.Api.Attributes;
using Orion.Api.AutoMapper.Input;
using Orion.Api.AutoMapper.Output;
using Orion.Entities.Domain;
using Orion.Entities.Filter;
using Orion.Entities.ValueObjects.Pagination;
using Orion.Domain.Interfaces;
using static Orion.Domain.Authentication.AuthorizationConfiguration;

namespace Orion.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [AuthorizeFor(Roles.Admin, Roles.Customer)]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService, IMapper mapper) : base(mapper)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] CustomerFilter filter)
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
            var customer = await _customerService.FindByIdAsync(id);
            var customerOutput = Mapper.Map<CustomerOutput>(customer);

            return Ok(customerOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerInput customerInput)
        {
            var customer = Mapper.Map<Customer>(customerInput);

            var created = await _customerService.AddAsync(customer);

            return Created(Mapper.Map<CustomerOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody]  CustomerInput customerInput)
        {
            customerInput.PublicId = id;
            var customer = Mapper.Map<Customer>(customerInput);

            await _customerService.UpdateAsync(customer);

            return Accepted();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            await _customerService.DeleteAsync(id);

            return NoContent();
        }
    }
}
