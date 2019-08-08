using AutoMapper;
using Invest.Api.AutoMapper;
using Invest.Entities.Domain;
using Invest.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Invest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ApiController
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService, IMapper mapper) : base(mapper)
        {
            _assetService = assetService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var asset = await _assetService.GetAll();

            var assetOutputList = _mapper.Map<IEnumerable<AssetOutput>>(asset);

            return Ok(assetOutputList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var asset = await _assetService.FindByIdAsync(id);
            var assetOutput = _mapper.Map<AssetOutput>(asset);

            return Ok(assetOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] AssetInput assetInput)
        {
            var asset = _mapper.Map<Asset>(assetInput);

            var created = await _assetService.AddAsync(asset);

            return Created("{id}", _mapper.Map<AssetOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody]  AssetInput assetInput)
        {
            assetInput.PublicId = id;
            var asset = _mapper.Map<Asset>(assetInput);

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
