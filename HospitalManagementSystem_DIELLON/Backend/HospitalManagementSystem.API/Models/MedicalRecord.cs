namespace HospitalManagementSystem.API.Models;

public class MedicalRecord
{
    public int RecordId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    
    // Foreign keys
    public int PatientId { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public ICollection<ClinicalEntry> ClinicalEntries { get; set; } = new List<ClinicalEntry>();
}


