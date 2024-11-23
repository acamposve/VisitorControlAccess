using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorsController : ControllerBase
    {
        private readonly IVisitorService _visitorService;

        public VisitorsController(IVisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetAllVisitors()
        {
            var visitors = await _visitorService.GetAllVisitorsAsync();
            return Ok(visitors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Visitor>> GetVisitorById(int id)
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            return Ok(visitor);
        }

        [HttpPost]
        public async Task<ActionResult<Visitor>> CreateVisitor(Visitor visitor)
        {
            var newVisitorId = await _visitorService.CreateVisitorAsync(visitor);
            return CreatedAtAction(nameof(GetVisitorById), new { id = newVisitorId }, visitor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisitor(int id, Visitor visitor)
        {
            if (id != visitor.Id)
            {
                return BadRequest();
            }

            var success = await _visitorService.UpdateVisitorAsync(visitor);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            var success = await _visitorService.DeleteVisitorAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
