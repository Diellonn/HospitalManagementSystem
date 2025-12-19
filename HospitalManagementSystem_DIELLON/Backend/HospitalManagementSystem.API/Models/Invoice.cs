namespace HospitalManagementSystem.API.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled
    public DateTime DateIssued { get; set; } = DateTime.UtcNow;
    public DateTime? DatePaid { get; set; }
    
    // Foreign keys
    public int PatientId { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}


