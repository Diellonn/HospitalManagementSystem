using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Services;

public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _context;

    public DoctorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorResponse> AddDoctorAsync(AddDoctorRequest request)
    {
        var doctor = new Doctor
        {
            Name = request.Name,
            Specialization = request.Specialization,
            ConsultationFee = request.ConsultationFee,
            Email = request.Email,
            Phone = request.Phone,
            Availability = request.Availability,
            DepartmentId = request.DepartmentId
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return await GetDoctorByIdAsync(doctor.DoctorId) ?? 
            throw new InvalidOperationException("Failed to retrieve created doctor");
    }

    public async Task<DoctorResponse?> GetDoctorByIdAsync(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.DoctorId == id);

        if (doctor == null) return null;

        return new DoctorResponse
        {
            DoctorId = doctor.DoctorId,
            Name = doctor.Name,
            Specialization = doctor.Specialization,
            ConsultationFee = doctor.ConsultationFee,
            Email = doctor.Email,
            Phone = doctor.Phone,
            Availability = doctor.Availability,
            DepartmentId = doctor.DepartmentId,
            DepartmentName = doctor.Department.Name
        };
    }

    public async Task<List<DoctorResponse>> GetAllDoctorsAsync()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Department)
            .ToListAsync();

        return doctors.Select(d => new DoctorResponse
        {
            DoctorId = d.DoctorId,
            Name = d.Name,
            Specialization = d.Specialization,
            ConsultationFee = d.ConsultationFee,
            Email = d.Email,
            Phone = d.Phone,
            Availability = d.Availability,
            DepartmentId = d.DepartmentId,
            DepartmentName = d.Department.Name
        }).ToList();
    }

    public async Task<DoctorResponse?> UpdateDoctorAsync(int id, UpdateDoctorRequest request)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return null;

        if (request.Name != null) doctor.Name = request.Name;
        if (request.Specialization != null) doctor.Specialization = request.Specialization;
        if (request.ConsultationFee.HasValue) doctor.ConsultationFee = request.ConsultationFee.Value;
        if (request.Email != null) doctor.Email = request.Email;
        if (request.Phone != null) doctor.Phone = request.Phone;
        if (request.Availability != null) doctor.Availability = request.Availability;
        if (request.DepartmentId.HasValue) doctor.DepartmentId = request.DepartmentId.Value;

        await _context.SaveChangesAsync();
        return await GetDoctorByIdAsync(id);
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return false;

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<DoctorResponse>> GetDoctorsByDepartmentAsync(int departmentId)
    {
        var doctors = await _context.Doctors
            .Include(d => d.Department)
            .Where(d => d.DepartmentId == departmentId)
            .ToListAsync();

        return doctors.Select(d => new DoctorResponse
        {
            DoctorId = d.DoctorId,
            Name = d.Name,
            Specialization = d.Specialization,
            ConsultationFee = d.ConsultationFee,
            Email = d.Email,
            Phone = d.Phone,
            Availability = d.Availability,
            DepartmentId = d.DepartmentId,
            DepartmentName = d.Department.Name
        }).ToList();
    }

    public async Task<bool> CheckDoctorAvailabilityAsync(int doctorId, DateTime appointmentTime)
    {
        var existingAppointment = await _context.Appointments
            .Where(a => a.DoctorId == doctorId 
                && a.Time == appointmentTime 
                && a.Status != "Cancelled")
            .FirstOrDefaultAsync();

        return existingAppointment == null;
    }
}


