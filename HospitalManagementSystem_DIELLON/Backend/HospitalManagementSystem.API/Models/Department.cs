namespace HospitalManagementSystem.API.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    public ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}


