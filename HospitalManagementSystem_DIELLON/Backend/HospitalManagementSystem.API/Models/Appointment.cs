namespace HospitalManagementSystem.API.Models;

public class Appointment
{
    public int AppointmentId { get; set; }
    public DateTime Time { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign keys
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}


