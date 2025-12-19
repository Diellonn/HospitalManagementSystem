namespace HospitalManagementSystem.API.Models;

public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ConsultationFee { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Availability { get; set; }
    
    // Foreign keys
    public int DepartmentId { get; set; }
    
    // Navigation properties
    public Department Department { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}


