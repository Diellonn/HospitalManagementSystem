namespace HospitalManagementSystem.API.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Bank Transfer, etc.
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    // Foreign keys
    public int InvoiceId { get; set; }
    
    // Navigation properties
    public Invoice Invoice { get; set; } = null!;
}


