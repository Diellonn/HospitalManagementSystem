using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IBillingService _billingService;

    public InvoiceController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> GenerateInvoice([FromBody] GenerateInvoiceRequest request)
    {
        try
        {
            var invoice = await _billingService.GenerateInvoiceAsync(request);
            return Ok(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<InvoiceDto>>> GetAllInvoices()
    {
        var invoices = await _billingService.GetAllInvoicesAsync();
        return Ok(invoices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var invoice = await _billingService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<InvoiceDto>>> GetInvoicesByPatient(int patientId)
    {
        var invoices = await _billingService.GetInvoicesByPatientIdAsync(patientId);
        return Ok(invoices);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoiceStatus(int id, [FromBody] UpdateInvoiceStatusRequest request)
    {
        var invoice = await _billingService.UpdateInvoiceStatusAsync(id, request.Status);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost("payment")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        try
        {
            var payment = await _billingService.ProcessPaymentAsync(request);
            return Ok(payment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{invoiceId}/payments")]
    public async Task<ActionResult<List<PaymentDto>>> GetPayments(int invoiceId)
    {
        var payments = await _billingService.GetPaymentsByInvoiceIdAsync(invoiceId);
        return Ok(payments);
    }
}


