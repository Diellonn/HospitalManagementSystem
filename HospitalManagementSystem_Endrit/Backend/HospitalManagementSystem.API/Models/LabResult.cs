namespace HospitalManagementSystem.API.Models;

public class LabResult
{
    public int ResultId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ResultData { get; set; }
    public DateTime TestDate { get; set; } = DateTime.UtcNow;
    public DateTime? ResultDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    
    // Foreign keys
    public int PatientId { get; set; }
    public int? DoctorId { get; set; }
    public int? NurseId { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Doctor? Doctor { get; set; }
    public Nurse? Nurse { get; set; }
}


