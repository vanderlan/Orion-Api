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
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService, IMapper mapper) : base(mapper)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] UserFilter filter)
        {
            var user = await _userService.ListPaginateAsync(filter);

            var userOutputList = _mapper.Map<PagedList<UserOutput>>(user);

            return Ok(userOutputList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.FindByIdAsync(id);
            var userOutput = _mapper.Map<UserOutput>(user);

            return Ok(userOutput);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] UserInput userInput)
        {
            var user = _mapper.Map<User>(userInput);

            var created = await _userService.AddAsync(user);

            return Created(_mapper.Map<UserOutput>(created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> Put(string id, [FromBody] UserInput userInput)
        {
            userInput.PublicId = id;
            var user = _mapper.Map<User>(userInput);

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
}
