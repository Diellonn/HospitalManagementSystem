namespace HospitalManagementSystem.API.DTOs;

public class AddRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string? RoleName { get; set; }
    public string? Permissions { get; set; }
}

public class RoleResponse
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public int UserCount { get; set; }
}


