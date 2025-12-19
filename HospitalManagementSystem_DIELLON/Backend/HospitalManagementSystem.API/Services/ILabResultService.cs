using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface ILabResultService
{
    Task<LabResultDto> AddLabResultAsync(AddLabResultRequest request);
    Task<LabResultDto?> GetLabResultByIdAsync(int id);
    Task<List<LabResultDto>> GetAllLabResultsAsync();
    Task<List<LabResultDto>> GetLabResultsByPatientIdAsync(int patientId);
    Task<LabResultDto?> UpdateLabResultAsync(int id, UpdateLabResultRequest request);
    Task<bool> DeleteLabResultAsync(int id);
}


