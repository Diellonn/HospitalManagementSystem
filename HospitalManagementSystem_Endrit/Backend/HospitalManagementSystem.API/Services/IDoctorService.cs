using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface IDoctorService
{
    Task<DoctorResponse> AddDoctorAsync(AddDoctorRequest request);
    Task<DoctorResponse?> GetDoctorByIdAsync(int id);
    Task<List<DoctorResponse>> GetAllDoctorsAsync();
    Task<DoctorResponse?> UpdateDoctorAsync(int id, UpdateDoctorRequest request);
    Task<bool> DeleteDoctorAsync(int id);
    Task<List<DoctorResponse>> GetDoctorsByDepartmentAsync(int departmentId);
    Task<bool> CheckDoctorAvailabilityAsync(int doctorId, DateTime appointmentTime);
}


