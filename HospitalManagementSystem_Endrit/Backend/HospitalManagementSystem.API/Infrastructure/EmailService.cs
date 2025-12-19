namespace HospitalManagementSystem.API.Infrastructure;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: Implement actual email sending logic (SMTP, SendGrid, etc.)
        _logger.LogInformation($"Email sent to {to}: {subject}");
        await Task.CompletedTask;
    }

    public async Task SendAppointmentConfirmationAsync(string patientEmail, string patientName, DateTime appointmentTime, string doctorName)
    {
        var subject = "Appointment Confirmation";
        var body = $@"
Dear {patientName},

Your appointment has been confirmed:
- Doctor: {doctorName}
- Date and Time: {appointmentTime:yyyy-MM-dd HH:mm}

Thank you for choosing our hospital.

Best regards,
Hospital Management System
";
        await SendEmailAsync(patientEmail, subject, body);
    }

    public async Task SendInvoiceAsync(string patientEmail, string patientName, string invoiceNumber, decimal total)
    {
        var subject = "Invoice from Hospital";
        var body = $@"
Dear {patientName},

Please find your invoice details:
- Invoice Number: {invoiceNumber}
- Total Amount: {total:C}

Thank you for your payment.

Best regards,
Hospital Management System
";
        await SendEmailAsync(patientEmail, subject, body);
    }

    public async Task SendLabResultNotificationAsync(string patientEmail, string patientName, string labResultType)
    {
        var subject = "Lab Results Available";
        var body = $@"
Dear {patientName},

Your lab results for {labResultType} are now available.
Please contact your doctor to review the results.

Best regards,
Hospital Management System
";
        await SendEmailAsync(patientEmail, subject, body);
    }
}


