namespace HospitalManagementSystem.API.Models;

public class Nurse
{
    public int NurseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // Foreign keys
    public int DepartmentId { get; set; }
    
    // Navigation properties
    public Department Department { get; set; } = null!;
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}


