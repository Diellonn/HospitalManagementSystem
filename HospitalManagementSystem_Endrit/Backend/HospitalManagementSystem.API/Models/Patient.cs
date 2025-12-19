namespace HospitalManagementSystem.API.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Insurance { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    
    // Foreign keys
    public int? DoctorId { get; set; }
    
    // Navigation properties
    public Doctor? Doctor { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public MedicalRecord? MedicalRecord { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}


