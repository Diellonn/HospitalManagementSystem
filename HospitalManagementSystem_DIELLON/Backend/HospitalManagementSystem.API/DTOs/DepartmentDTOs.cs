namespace HospitalManagementSystem.API.DTOs;

public class AddDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateDepartmentRequest
{
    public string? Name { get; set; }
}

public class DepartmentResponse
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int NurseCount { get; set; }
    public int RoomCount { get; set; }
}


