using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechTestKLG.DTO;
using TechTestKLG.Models;
using TechTestKLG.Services;
using TechTestKLG.Services.Interfaces;

namespace TechTestKLG.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        public ActivityController(IMapper mapper, IActivityService activityService)
        {
            _mapper = mapper;
            _activityService = activityService;
        }



        [HttpPost("GetActivity")]
        public async Task<IActionResult> Get(GridviewRequestDTO request)
        {
            var data = await _activityService.GetActivity(request);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var activity = await _activityService.GetActivity(id);
            return activity == null ? NotFound() : Ok(activity);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] ActivityDTO input)
        {
            try
            {
                var activity = _mapper.Map<Activities>(input);
                var success = await _activityService.Insert(activity);
                return success ? Ok(activity) : BadRequest("Activity insert failed.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during insertion: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] ActivityDTO input)
        {
            try
            {
                var activity = _mapper.Map<Activities>(input);
                var success = await _activityService.Update(activity);
                return success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during update: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _activityService.Delete(id);
            return success ? Ok() : NotFound();
        }
    }
}
