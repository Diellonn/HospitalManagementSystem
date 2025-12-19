using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface IPatientService
{
    Task<PatientResponse> AddPatientAsync(AddPatientRequest request);
    Task<PatientResponse?> GetPatientByIdAsync(int id);
    Task<List<PatientResponse>> GetAllPatientsAsync();
    Task<PatientResponse?> UpdatePatientAsync(int id, UpdatePatientRequest request);
    Task<bool> DeletePatientAsync(int id);
}


