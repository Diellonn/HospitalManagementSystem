using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface IAppointmentService
{
    Task<AppointmentDetailsDto> CreateAppointmentAsync(CreateAppointmentRequest request);
    Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id);
    Task<List<AppointmentDetailsDto>> GetAllAppointmentsAsync();
    Task<List<AppointmentDetailsDto>> GetAppointmentsByPatientAsync(int patientId);
    Task<List<AppointmentDetailsDto>> GetAppointmentsByDoctorAsync(int doctorId);
    Task<AppointmentDetailsDto?> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request);
    Task<bool> CancelAppointmentAsync(int id);
    Task<bool> CheckAvailabilityAsync(int doctorId, DateTime appointmentTime);
}


