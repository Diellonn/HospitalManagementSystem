namespace HospitalManagementSystem.API.Models;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
}


