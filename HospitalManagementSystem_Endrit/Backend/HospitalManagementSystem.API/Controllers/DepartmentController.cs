using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> AddDepartment([FromBody] AddDepartmentRequest request)
    {
        var department = new Department
        {
            Name = request.Name
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return Ok(new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            DoctorCount = 0,
            NurseCount = 0,
            RoomCount = 0
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<DepartmentResponse>>> GetAllDepartments()
    {
        var departments = await _context.Departments
            .Include(d => d.Doctors)
            .Include(d => d.Nurses)
            .Include(d => d.Rooms)
            .ToListAsync();

        return Ok(departments.Select(d => new DepartmentResponse
        {
            DepartmentId = d.DepartmentId,
            Name = d.Name,
            DoctorCount = d.Doctors.Count,
            NurseCount = d.Nurses.Count,
            RoomCount = d.Rooms.Count
        }).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Doctors)
            .Include(d => d.Nurses)
            .Include(d => d.Rooms)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);

        if (department == null) return NotFound();

        return Ok(new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            DoctorCount = department.Doctors.Count,
            NurseCount = department.Nurses.Count,
            RoomCount = department.Rooms.Count
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentResponse>> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        if (request.Name != null) department.Name = request.Name;

        await _context.SaveChangesAsync();

        return Ok(new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name,
            DoctorCount = await _context.Doctors.CountAsync(d => d.DepartmentId == id),
            NurseCount = await _context.Nurses.CountAsync(n => n.DepartmentId == id),
            RoomCount = await _context.Rooms.CountAsync(r => r.DepartmentId == id)
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Doctors)
            .Include(d => d.Nurses)
            .Include(d => d.Rooms)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
        
        if (department == null) return NotFound();

        // Delete all related doctors first (and their related records)
        foreach (var doctor in department.Doctors.ToList())
        {
            // Delete doctor's appointments
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctor.DoctorId)
                .ToListAsync();
            _context.Appointments.RemoveRange(appointments);

            // Delete doctor's prescriptions
            var prescriptions = await _context.Prescriptions
                .Where(p => p.DoctorId == doctor.DoctorId)
                .ToListAsync();
            _context.Prescriptions.RemoveRange(prescriptions);

            // Update lab results to set doctor to null (already configured with SetNull)
            var labResults = await _context.LabResults
                .Where(l => l.DoctorId == doctor.DoctorId)
                .ToListAsync();
            foreach (var labResult in labResults)
            {
                labResult.DoctorId = null;
            }

            // Update patients to set doctor to null (already configured with SetNull)
            var patients = await _context.Patients
                .Where(p => p.DoctorId == doctor.DoctorId)
                .ToListAsync();
            foreach (var patient in patients)
            {
                patient.DoctorId = null;
            }

            _context.Doctors.Remove(doctor);
        }

        // Delete all related nurses first
        foreach (var nurse in department.Nurses.ToList())
        {
            // Update lab results to set nurse to null (already configured with SetNull)
            var labResults = await _context.LabResults
                .Where(l => l.NurseId == nurse.NurseId)
                .ToListAsync();
            foreach (var labResult in labResults)
            {
                labResult.NurseId = null;
            }

            _context.Nurses.Remove(nurse);
        }

        // Delete all related rooms
        _context.Rooms.RemoveRange(department.Rooms);

        // Now delete the department
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/rooms")]
    public async Task<ActionResult> GetRoomsByDepartment(int id)
    {
        var rooms = await _context.Rooms
            .Where(r => r.DepartmentId == id)
            .ToListAsync();

        return Ok(rooms.Select(r => new
        {
            r.RoomId,
            r.Type,
            r.Status,
            r.DepartmentId
        }));
    }
}


