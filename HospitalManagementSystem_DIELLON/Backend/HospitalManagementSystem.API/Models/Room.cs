namespace HospitalManagementSystem.API.Models;

public class Room
{
    public int RoomId { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Status { get; set; } = true; // true = available, false = occupied
    
    // Foreign keys
    public int DepartmentId { get; set; }
    
    // Navigation properties
    public Department Department { get; set; } = null!;
}


