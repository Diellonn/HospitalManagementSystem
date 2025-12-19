namespace HospitalManagementSystem.API.Infrastructure;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendAppointmentConfirmationAsync(string patientEmail, string patientName, DateTime appointmentTime, string doctorName);
    Task SendInvoiceAsync(string patientEmail, string patientName, string invoiceNumber, decimal total);
    Task SendLabResultNotificationAsync(string patientEmail, string patientName, string labResultType);
}


