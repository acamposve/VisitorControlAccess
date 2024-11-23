using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BraceletsController : ControllerBase
    {
        private readonly IBraceletService _braceletService;

        public BraceletsController(IBraceletService braceletService)
        {
            _braceletService = braceletService;
        }

        // GET api/bracelets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bracelet>>> GetAll()
        {
            var bracelets = await _braceletService.GetAllAsync();
            return Ok(bracelets);
        }

        // GET api/bracelets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bracelet>> GetById(int id)
        {
            var bracelet = await _braceletService.GetByIdAsync(id);
            if (bracelet == null)
            {
                return NotFound();
            }
            return Ok(bracelet);
        }

        // POST api/bracelets
        [HttpPost]
        public async Task<ActionResult<Bracelet>> Create([FromBody] Bracelet bracelet)
        {
            if (bracelet == null)
            {
                return BadRequest();
            }

            var id = await _braceletService.CreateAsync(bracelet);
            bracelet.Id = id;

            return CreatedAtAction(nameof(GetById), new { id = bracelet.Id }, bracelet);
        }

        // PUT api/bracelets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Bracelet bracelet)
        {
            if (bracelet == null || bracelet.Id != id)
            {
                return BadRequest();
            }

            var existingBracelet = await _braceletService.GetByIdAsync(id);
            if (existingBracelet == null)
            {
                return NotFound();
            }

            await _braceletService.UpdateAsync(bracelet);
            return NoContent();
        }

        // DELETE api/bracelets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bracelet = await _braceletService.GetByIdAsync(id);
            if (bracelet == null)
            {
                return NotFound();
            }

            await _braceletService.DeleteAsync(id);
            return NoContent();
        }
    }
}
