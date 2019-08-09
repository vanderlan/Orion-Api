using AutoMapper;
using VBaseProject.Api.AutoMapper;
using VBaseProject.Entities.Domain;
using VBaseProject.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace VBaseProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _assetService;

        public CustomersController(ICustomerService assetService, IMapper mapper) : base(mapper)
        {
            _assetService = assetService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var asset = await _assetService.GetAll();

            var assetOutputList = _mapper.Map<IEnumerable<CustomerOutput>>(asset);

            return Ok(assetOutputList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var asset = await _assetService.FindByIdAsync(id);
            var assetOutput = _mapper.Map<CustomerOutput>(asset);

            return Ok(assetOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerInput assetInput)
        {
            var asset = _mapper.Map<Customer>(assetInput);

            var created = await _assetService.AddAsync(asset);

            return Created("{id}", _mapper.Map<CustomerOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody]  CustomerInput assetInput)
        {
            assetInput.PublicId = id;
            var asset = _mapper.Map<Customer>(assetInput);

            await _assetService.UpdateAsync(asset);

            return Accepted();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _assetService.DeleteAsync(id);

            return NoContent();
        }
    }
}
