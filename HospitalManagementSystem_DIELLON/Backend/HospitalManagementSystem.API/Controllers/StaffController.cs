using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ApplicationDbContext _context;

    public StaffController(IDoctorService doctorService, ApplicationDbContext context)
    {
        _doctorService = doctorService;
        _context = context;
    }

    // Doctor endpoints
    [HttpPost("doctors")]
    public async Task<ActionResult<DoctorResponse>> AddDoctor([FromBody] AddDoctorRequest request)
    {
        try
        {
            var doctor = await _doctorService.AddDoctorAsync(request);
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("doctors")]
    public async Task<ActionResult<List<DoctorResponse>>> GetAllDoctors()
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        return Ok(doctors);
    }

    [HttpGet("doctors/{id}")]
    public async Task<ActionResult<DoctorResponse>> GetDoctor(int id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        if (doctor == null) return NotFound();
        return Ok(doctor);
    }

    [HttpGet("doctors/department/{departmentId}")]
    public async Task<ActionResult<List<DoctorResponse>>> GetDoctorsByDepartment(int departmentId)
    {
        var doctors = await _doctorService.GetDoctorsByDepartmentAsync(departmentId);
        return Ok(doctors);
    }

    [HttpPut("doctors/{id}")]
    public async Task<ActionResult<DoctorResponse>> UpdateDoctor(int id, [FromBody] UpdateDoctorRequest request)
    {
        var doctor = await _doctorService.UpdateDoctorAsync(id, request);
        if (doctor == null) return NotFound();
        return Ok(doctor);
    }

    [HttpDelete("doctors/{id}")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var result = await _doctorService.DeleteDoctorAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    // Nurse endpoints
    [HttpPost("nurses")]
    public async Task<ActionResult> AddNurse([FromBody] AddNurseRequest request)
    {
        var nurse = new Nurse
        {
            Name = request.Name,
            Ward = request.Ward,
            Email = request.Email,
            Phone = request.Phone,
            DepartmentId = request.DepartmentId
        };

        _context.Nurses.Add(nurse);
        await _context.SaveChangesAsync();

        return Ok(new { NurseId = nurse.NurseId, Name = nurse.Name, Ward = nurse.Ward });
    }

    [HttpGet("nurses")]
    public async Task<ActionResult> GetAllNurses()
    {
        var nurses = await _context.Nurses
            .Include(n => n.Department)
            .ToListAsync();

        return Ok(nurses.Select(n => new
        {
            n.NurseId,
            n.Name,
            n.Ward,
            n.Email,
            n.Phone,
            DepartmentId = n.DepartmentId,
            DepartmentName = n.Department.Name
        }));
    }

    [HttpGet("nurses/{id}")]
    public async Task<ActionResult> GetNurse(int id)
    {
        var nurse = await _context.Nurses
            .Include(n => n.Department)
            .FirstOrDefaultAsync(n => n.NurseId == id);

        if (nurse == null) return NotFound();

        return Ok(new
        {
            nurse.NurseId,
            nurse.Name,
            nurse.Ward,
            nurse.Email,
            nurse.Phone,
            DepartmentId = nurse.DepartmentId,
            DepartmentName = nurse.Department.Name
        });
    }
}

// DTOs for Nurse
public class AddNurseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int DepartmentId { get; set; }
}


