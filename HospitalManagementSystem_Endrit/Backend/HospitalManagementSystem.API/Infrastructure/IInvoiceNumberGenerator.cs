namespace HospitalManagementSystem.API.Infrastructure;

public interface IInvoiceNumberGenerator
{
    Task<string> GenerateInvoiceNumberAsync();
}


