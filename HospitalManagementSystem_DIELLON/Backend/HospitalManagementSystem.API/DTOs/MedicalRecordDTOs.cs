namespace HospitalManagementSystem.API.DTOs;

public class AddMedicalRecordRequest
{
    public int PatientId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
}

public class UpdateMedicalRecordRequest
{
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
}

public class AddClinicalEntryRequest
{
    public int RecordId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
}

public class MedicalRecordDto
{
    public int RecordId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public List<ClinicalEntryDto> ClinicalEntries { get; set; } = new();
}

public class ClinicalEntryDto
{
    public int EntryId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
}


