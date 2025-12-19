namespace HospitalManagementSystem.API.DTOs;

public class CreateAppointmentRequest
{
    public DateTime Time { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
}

public class UpdateAppointmentRequest
{
    public DateTime? Time { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
}

public class AppointmentDetailsDto
{
    public int AppointmentId { get; set; }
    public DateTime Time { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialization { get; set; } = string.Empty;
}


