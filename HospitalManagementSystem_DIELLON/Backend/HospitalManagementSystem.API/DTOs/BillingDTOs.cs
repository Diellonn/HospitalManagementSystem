namespace HospitalManagementSystem.API.DTOs;

public class GenerateInvoiceRequest
{
    public int PatientId { get; set; }
    public decimal Total { get; set; }
    public string? Description { get; set; }
}

public class UpdateInvoiceStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class ProcessPaymentRequest
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
}

public class InvoiceDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateIssued { get; set; }
    public DateTime? DatePaid { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public List<PaymentDto> Payments { get; set; } = new();
}

public class PaymentDto
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public int InvoiceId { get; set; }
}


