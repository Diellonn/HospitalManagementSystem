namespace HospitalManagementSystem.API.Models;

public class ClinicalEntry
{
    public int EntryId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
    public string? Diagnosis { get; set; }
    
    // Foreign keys
    public int RecordId { get; set; }
    
    // Navigation properties
    public MedicalRecord MedicalRecord { get; set; } = null!;
}


