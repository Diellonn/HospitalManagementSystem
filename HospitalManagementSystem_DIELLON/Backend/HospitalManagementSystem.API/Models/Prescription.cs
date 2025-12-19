namespace HospitalManagementSystem.API.Models;

public class Prescription
{
    public int PrescriptionId { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? Medication { get; set; }
    public string? Dosage { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    
    // Foreign keys
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    
    // Navigation properties
    public Doctor Doctor { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}


