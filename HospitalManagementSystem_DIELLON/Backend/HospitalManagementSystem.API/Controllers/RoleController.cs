using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoleController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> AddRole([FromBody] AddRoleRequest request)
    {
        if (await _context.Roles.AnyAsync(r => r.RoleName == request.RoleName))
        {
            return BadRequest("Role already exists");
        }

        var role = new Role
        {
            RoleName = request.RoleName,
            Permissions = request.Permissions
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return Ok(new RoleResponse
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Permissions = role.Permissions,
            UserCount = 0
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleResponse>>> GetAllRoles()
    {
        var roles = await _context.Roles.ToListAsync();

        return Ok(roles.Select(r => new RoleResponse
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            Permissions = r.Permissions,
            UserCount = 0
        }).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleResponse>> GetRole(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == id);

        if (role == null) return NotFound();

        return Ok(new RoleResponse
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Permissions = role.Permissions,
            UserCount = 0
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleResponse>> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();

        if (request.RoleName != null) role.RoleName = request.RoleName;
        if (request.Permissions != null) role.Permissions = request.Permissions;

        await _context.SaveChangesAsync();

        return Ok(new RoleResponse
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Permissions = role.Permissions,
            UserCount = 0
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
