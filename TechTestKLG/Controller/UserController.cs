using Microsoft.AspNetCore.Mvc;
using TechTestKLG.Models;
using TechTestKLG.DTO;
using TechTestKLG.Services.Interfaces;
using AutoMapper;

namespace TechTestKLG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("GetUsers")]
        public async Task<IActionResult> Get(GridviewRequestDTO request)
        {
            var users = await _userService.GetUsers(request);
            return Ok(users);
        }

        [HttpGet("getuser/{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var user = await _userService.GetUser(username);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] UserDTO userDto)
        {
            try
            {
                var user = _mapper.Map<Users>(userDto);
                var success = await _userService.Insert(user);
                return success ? Ok(user) : BadRequest("User insert failed.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during insertion: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserDTO userDto)
        {
            try
            {
                var user = _mapper.Map<Users>(userDto);
                var success = await _userService.Update(user);
                return success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during update: {ex.Message}");
            }
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            var success = await _userService.Delete(username);
            return success ? Ok() : NotFound();
        }
    }
}
