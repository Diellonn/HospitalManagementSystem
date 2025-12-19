using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface IMedicalRecordService
{
    Task<MedicalRecordDto> AddMedicalRecordAsync(AddMedicalRecordRequest request);
    Task<MedicalRecordDto?> GetMedicalRecordByPatientIdAsync(int patientId);
    Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int recordId);
    Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int recordId, UpdateMedicalRecordRequest request);
    Task<ClinicalEntryDto> AddClinicalEntryAsync(AddClinicalEntryRequest request);
    Task<List<ClinicalEntryDto>> GetClinicalEntriesByRecordIdAsync(int recordId);
}


