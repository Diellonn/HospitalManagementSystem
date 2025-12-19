namespace HospitalManagementSystem.API.DTOs;

public class AddPatientRequest
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Insurance { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? DoctorId { get; set; }
}

public class UpdatePatientRequest
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Insurance { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? DoctorId { get; set; }
}

public class PatientResponse
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Insurance { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
}


