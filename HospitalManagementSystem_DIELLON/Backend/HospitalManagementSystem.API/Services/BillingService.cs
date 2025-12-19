using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Infrastructure;

namespace HospitalManagementSystem.API.Services;

public class BillingService : IBillingService
{
    private readonly ApplicationDbContext _context;
    private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
    private readonly ISimClock _simClock;
    private readonly IEmailService _emailService;

    public BillingService(
        ApplicationDbContext context, 
        IInvoiceNumberGenerator invoiceNumberGenerator,
        ISimClock simClock,
        IEmailService emailService)
    {
        _context = context;
        _invoiceNumberGenerator = invoiceNumberGenerator;
        _simClock = simClock;
        _emailService = emailService;
    }

    public async Task<InvoiceDto> GenerateInvoiceAsync(GenerateInvoiceRequest request)
    {
        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient == null)
        {
            throw new InvalidOperationException("Patient not found");
        }

        var invoice = new Invoice
        {
            InvoiceNumber = await _invoiceNumberGenerator.GenerateInvoiceNumberAsync(),
            PatientId = request.PatientId,
            Total = request.Total,
            Status = "Pending",
            DateIssued = _simClock.GetCurrentTime()
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Send invoice email
        if (!string.IsNullOrEmpty(patient.Email))
        {
            await _emailService.SendInvoiceAsync(
                patient.Email,
                patient.Name,
                invoice.InvoiceNumber,
                invoice.Total
            );
        }

        return await GetInvoiceByIdAsync(invoice.InvoiceId) ?? 
            throw new InvalidOperationException("Failed to retrieve created invoice");
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

        if (invoice == null) return null;

        return new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            Total = invoice.Total,
            Status = invoice.Status,
            DateIssued = invoice.DateIssued,
            DatePaid = invoice.DatePaid,
            PatientId = invoice.PatientId,
            PatientName = invoice.Patient.Name,
            Payments = invoice.Payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate,
                InvoiceId = p.InvoiceId
            }).ToList()
        };
    }

    public async Task<List<InvoiceDto>> GetAllInvoicesAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .ToListAsync();

        return invoices.Select(i => new InvoiceDto
        {
            InvoiceId = i.InvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            Total = i.Total,
            Status = i.Status,
            DateIssued = i.DateIssued,
            DatePaid = i.DatePaid,
            PatientId = i.PatientId,
            PatientName = i.Patient.Name,
            Payments = i.Payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate,
                InvoiceId = p.InvoiceId
            }).ToList()
        }).ToList();
    }

    public async Task<List<InvoiceDto>> GetInvoicesByPatientIdAsync(int patientId)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Payments)
            .Where(i => i.PatientId == patientId)
            .ToListAsync();

        return invoices.Select(i => new InvoiceDto
        {
            InvoiceId = i.InvoiceId,
            InvoiceNumber = i.InvoiceNumber,
            Total = i.Total,
            Status = i.Status,
            DateIssued = i.DateIssued,
            DatePaid = i.DatePaid,
            PatientId = i.PatientId,
            PatientName = i.Patient.Name,
            Payments = i.Payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate,
                InvoiceId = p.InvoiceId
            }).ToList()
        }).ToList();
    }

    public async Task<InvoiceDto?> UpdateInvoiceStatusAsync(int invoiceId, string status)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null) return null;

        invoice.Status = status;
        
        if (status == "Paid" && invoice.DatePaid == null)
        {
            invoice.DatePaid = _simClock.GetCurrentTime();
        }
        else if (status != "Paid")
        {
            invoice.DatePaid = null;
        }

        await _context.SaveChangesAsync();
        return await GetInvoiceByIdAsync(invoiceId);
    }

    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceId == request.InvoiceId);

        if (invoice == null)
        {
            throw new InvalidOperationException("Invoice not found");
        }

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            TransactionId = request.TransactionId,
            PaymentDate = _simClock.GetCurrentTime()
        };

        _context.Payments.Add(payment);

        // Update invoice status based on payments
        var totalPaid = invoice.Payments.Sum(p => p.Amount) + request.Amount;
        if (totalPaid >= invoice.Total)
        {
            invoice.Status = "Paid";
            invoice.DatePaid = _simClock.GetCurrentTime();
        }
        else
        {
            invoice.Status = "Partially Paid";
        }

        await _context.SaveChangesAsync();

        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            PaymentDate = payment.PaymentDate,
            InvoiceId = payment.InvoiceId
        };
    }

    public async Task<List<PaymentDto>> GetPaymentsByInvoiceIdAsync(int invoiceId)
    {
        var payments = await _context.Payments
            .Where(p => p.InvoiceId == invoiceId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return payments.Select(p => new PaymentDto
        {
            PaymentId = p.PaymentId,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            TransactionId = p.TransactionId,
            PaymentDate = p.PaymentDate,
            InvoiceId = p.InvoiceId
        }).ToList();
    }
}


