namespace HospitalManagementSystem.API.DTOs;

public class AddDoctorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ConsultationFee { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Availability { get; set; }
    public int DepartmentId { get; set; }
}

public class UpdateDoctorRequest
{
    public string? Name { get; set; }
    public string? Specialization { get; set; }
    public int? ConsultationFee { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Availability { get; set; }
    public int? DepartmentId { get; set; }
}

public class DoctorResponse
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ConsultationFee { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Availability { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}


