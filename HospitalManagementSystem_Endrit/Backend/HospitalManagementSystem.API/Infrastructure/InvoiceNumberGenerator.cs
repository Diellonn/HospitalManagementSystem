using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;

namespace HospitalManagementSystem.API.Infrastructure;

public class InvoiceNumberGenerator : IInvoiceNumberGenerator
{
    private readonly ISimClock _simClock;
    private readonly ApplicationDbContext _context;

    public InvoiceNumberGenerator(ISimClock simClock, ApplicationDbContext context)
    {
        _simClock = simClock;
        _context = context;
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var date = _simClock.GetCurrentDate();
        var year = date.Year;
        var month = date.Month.ToString("D2");
        var prefix = $"INV-{year}{month}-";

        // Find the highest sequence number for this month
        // Using a more reliable method: extract numeric part and find max
        var invoicesThisMonth = await _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .Select(i => i.InvoiceNumber)
            .ToListAsync();

        int maxSequence = 0;
        foreach (var invoiceNumber in invoicesThisMonth)
        {
            if (invoiceNumber.Length > prefix.Length)
            {
                var sequencePart = invoiceNumber.Substring(prefix.Length);
                if (int.TryParse(sequencePart, out int sequence))
                {
                    if (sequence > maxSequence)
                    {
                        maxSequence = sequence;
                    }
                }
            }
        }

        int nextSequence = maxSequence + 1;
        var sequenceString = nextSequence.ToString("D6");
        return $"{prefix}{sequenceString}";
    }
}


