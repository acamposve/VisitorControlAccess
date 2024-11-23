using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET api/roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetAll()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    // GET api/roles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    // POST api/roles
    [HttpPost]
    public async Task<ActionResult<Role>> Create([FromBody] Role role)
    {
        if (role == null)
        {
            return BadRequest();
        }

        var id = await _roleService.CreateRoleAsync(role);
        role.Id = id;

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    // PUT api/roles/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        if (role == null || role.Id != id)
        {
            return BadRequest();
        }

        var existingRole = await _roleService.GetRoleByIdAsync(id);
        if (existingRole == null)
        {
            return NotFound();
        }

        await _roleService.UpdateRoleAsync(role);
        return NoContent();
    }

    // DELETE api/roles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
