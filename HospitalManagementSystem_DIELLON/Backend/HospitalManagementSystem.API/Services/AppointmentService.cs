using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Infrastructure;

namespace HospitalManagementSystem.API.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ISimClock _simClock;
    private readonly IEmailService _emailService;

    public AppointmentService(ApplicationDbContext context, ISimClock simClock, IEmailService emailService)
    {
        _context = context;
        _simClock = simClock;
        _emailService = emailService;
    }

    public async Task<AppointmentDetailsDto> CreateAppointmentAsync(CreateAppointmentRequest request)
    {
        // Check if doctor is available
        var isAvailable = await CheckAvailabilityAsync(request.DoctorId, request.Time);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Doctor is not available at the requested time");
        }

        // Verify patient exists
        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient == null)
        {
            throw new InvalidOperationException("Patient not found");
        }

        var appointment = new Appointment
        {
            Time = request.Time,
            Reason = request.Reason,
            Status = "Scheduled",
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            CreatedAt = _simClock.GetCurrentTime()
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Send confirmation email
        if (!string.IsNullOrEmpty(patient.Email))
        {
            var doctor = await _context.Doctors.FindAsync(request.DoctorId);
            await _emailService.SendAppointmentConfirmationAsync(
                patient.Email, 
                patient.Name, 
                request.Time, 
                doctor?.Name ?? "Doctor"
            );
        }

        return await GetAppointmentByIdAsync(appointment.AppointmentId) ?? 
            throw new InvalidOperationException("Failed to retrieve created appointment");
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);

        if (appointment == null) return null;

        return new AppointmentDetailsDto
        {
            AppointmentId = appointment.AppointmentId,
            Time = appointment.Time,
            Status = appointment.Status,
            Reason = appointment.Reason,
            CreatedAt = appointment.CreatedAt,
            PatientId = appointment.PatientId,
            PatientName = appointment.Patient.Name,
            DoctorId = appointment.DoctorId,
            DoctorName = appointment.Doctor.Name,
            DoctorSpecialization = appointment.Doctor.Specialization
        };
    }

    public async Task<List<AppointmentDetailsDto>> GetAllAppointmentsAsync()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .ToListAsync();

        return appointments.Select(a => new AppointmentDetailsDto
        {
            AppointmentId = a.AppointmentId,
            Time = a.Time,
            Status = a.Status,
            Reason = a.Reason,
            CreatedAt = a.CreatedAt,
            PatientId = a.PatientId,
            PatientName = a.Patient.Name,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor.Name,
            DoctorSpecialization = a.Doctor.Specialization
        }).ToList();
    }

    public async Task<List<AppointmentDetailsDto>> GetAppointmentsByPatientAsync(int patientId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .ToListAsync();

        return appointments.Select(a => new AppointmentDetailsDto
        {
            AppointmentId = a.AppointmentId,
            Time = a.Time,
            Status = a.Status,
            Reason = a.Reason,
            CreatedAt = a.CreatedAt,
            PatientId = a.PatientId,
            PatientName = a.Patient.Name,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor.Name,
            DoctorSpecialization = a.Doctor.Specialization
        }).ToList();
    }

    public async Task<List<AppointmentDetailsDto>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();

        return appointments.Select(a => new AppointmentDetailsDto
        {
            AppointmentId = a.AppointmentId,
            Time = a.Time,
            Status = a.Status,
            Reason = a.Reason,
            CreatedAt = a.CreatedAt,
            PatientId = a.PatientId,
            PatientName = a.Patient.Name,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor.Name,
            DoctorSpecialization = a.Doctor.Specialization
        }).ToList();
    }

    public async Task<AppointmentDetailsDto?> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return null;

        if (request.Time.HasValue) appointment.Time = request.Time.Value;
        if (request.Status != null) appointment.Status = request.Status;
        if (request.Reason != null) appointment.Reason = request.Reason;

        await _context.SaveChangesAsync();
        return await GetAppointmentByIdAsync(id);
    }

    public async Task<bool> CancelAppointmentAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return false;

        appointment.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckAvailabilityAsync(int doctorId, DateTime appointmentTime)
    {
        var existingAppointment = await _context.Appointments
            .Where(a => a.DoctorId == doctorId 
                && a.Time == appointmentTime 
                && a.Status != "Cancelled")
            .FirstOrDefaultAsync();

        return existingAppointment == null;
    }
}


