using AccessControlApi.Application.Services;
using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessPointsController : ControllerBase
    {
        private readonly IAccessPointService _accessPointService;

        public AccessPointsController(IAccessPointService accessPointService)
        {
            _accessPointService = accessPointService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accessPoints = await _accessPointService.GetAllAccessPointsAsync();
            return Ok(accessPoints);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var accessPoint = await _accessPointService.GetAccessPointByIdAsync(id);
            if (accessPoint == null)
            {
                return NotFound();
            }
            return Ok(accessPoint);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccessPoint accessPoint)
        {
            var id = await _accessPointService.CreateAccessPointAsync(accessPoint);
            return CreatedAtAction(nameof(Get), new { id = id }, accessPoint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AccessPoint accessPoint)
        {
            accessPoint.Id = id;
            var success = await _accessPointService.UpdateAccessPointAsync(accessPoint);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _accessPointService.DeleteAccessPointAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
