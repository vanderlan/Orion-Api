using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Threading.Tasks;
using VBaseProject.Api.AutoMapper.Input;
using VBaseProject.Api.AutoMapper.Output;
using VBaseProject.Entities.Domain;
using VBaseProject.Entities.Filter;
using VBaseProject.Entities.ValueObjects.Pagination;
using VBaseProject.Resources;
using VBaseProject.Service.Interfaces;

namespace VBaseProject.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    //[AuthorizeFor(Roles.Admin)]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _custumerService;
        private readonly IStringLocalizer<VBaseProjectResources> _stringLocalizer;

        public CustomersController(ICustomerService assetService, IMapper mapper, IStringLocalizer<VBaseProjectResources> stringLocalizer) : base(mapper)
        {
            _stringLocalizer = stringLocalizer;
            _custumerService = assetService;
        }

        [HttpGet]
        [Route("Olar")]
        public IActionResult GetCulture()
        {
            var msg = _stringLocalizer["InvalidCustomerName"];

            var eagora = msg.SearchedLocation;

            return Ok(msg);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] CustomerFilter filter)
        {

            var asset = await _custumerService.ListPaginate(filter);

            var assetOutputList = _mapper.Map<PagedList<CustomerOutput>>(asset);

            return Ok(assetOutputList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var asset = await _custumerService.FindByIdAsync(id);
            var assetOutput = _mapper.Map<CustomerOutput>(asset);

            return Ok(assetOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerInput assetInput)
        {
            var asset = _mapper.Map<Customer>(assetInput);

            var created = await _custumerService.AddAsync(asset);

            return Created(_mapper.Map<CustomerOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody]  CustomerInput assetInput)
        {
            assetInput.PublicId = id;
            var asset = _mapper.Map<Customer>(assetInput);

            await _custumerService.UpdateAsync(asset);

            return Accepted();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            await _custumerService.DeleteAsync(id);

            return NoContent();
        }
    }
}
