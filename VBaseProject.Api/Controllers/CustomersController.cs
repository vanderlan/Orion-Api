using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using VBaseProject.Api.Attributes;
using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Domain.Interfaces;
using static VBaseProject.Domain.Authentication.AuthorizationConfiguration;

namespace VBaseProject.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeFor(Roles.Admin)]
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
            var customer = await _customerService.ListPaginate(filter);

            var customerOutputList = _mapper.Map<PagedList<CustomerOutput>>(customer);

            return Ok(customerOutputList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var customer = await _customerService.FindByIdAsync(id);
            var customerOutput = _mapper.Map<CustomerOutput>(customer);

            return Ok(customerOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerInput customerInput)
        {
            var customer = _mapper.Map<Customer>(customerInput);

            var created = await _customerService.AddAsync(customer);

            return Created(_mapper.Map<CustomerOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody]  CustomerInput customerInput)
        {
            customerInput.PublicId = id;
            var customer = _mapper.Map<Customer>(customerInput);

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
