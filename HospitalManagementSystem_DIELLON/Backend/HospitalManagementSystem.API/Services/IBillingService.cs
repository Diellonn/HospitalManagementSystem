using HospitalManagementSystem.API.DTOs;

namespace HospitalManagementSystem.API.Services;

public interface IBillingService
{
    Task<InvoiceDto> GenerateInvoiceAsync(GenerateInvoiceRequest request);
    Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
    Task<List<InvoiceDto>> GetAllInvoicesAsync();
    Task<List<InvoiceDto>> GetInvoicesByPatientIdAsync(int patientId);
    Task<InvoiceDto?> UpdateInvoiceStatusAsync(int invoiceId, string status);
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentRequest request);
    Task<List<PaymentDto>> GetPaymentsByInvoiceIdAsync(int invoiceId);
}


