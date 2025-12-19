namespace HospitalManagementSystem.API.DTOs;

public class AddLabResultRequest
{
    public int PatientId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ResultData { get; set; }
    public DateTime TestDate { get; set; }
    public int? DoctorId { get; set; }
    public int? NurseId { get; set; }
}

public class UpdateLabResultRequest
{
    public string? ResultData { get; set; }
    public DateTime? ResultDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
}

public class LabResultDto
{
    public int ResultId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ResultData { get; set; }
    public DateTime TestDate { get; set; }
    public DateTime? ResultDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public int? NurseId { get; set; }
    public string? NurseName { get; set; }
}


