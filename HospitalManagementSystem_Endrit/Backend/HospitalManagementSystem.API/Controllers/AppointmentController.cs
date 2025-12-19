using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDetailsDto>> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(request);
            return Ok(appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAppointmentsByPatient(int patientId)
    {
        var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
        return Ok(appointments);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAppointmentsByDoctor(int doctorId)
    {
        var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
        return Ok(appointments);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request)
    {
        var appointment = await _appointmentService.UpdateAppointmentAsync(id, request);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("check-availability")]
    public async Task<ActionResult<bool>> CheckAvailability([FromQuery] int doctorId, [FromQuery] DateTime appointmentTime)
    {
        var isAvailable = await _appointmentService.CheckAvailabilityAsync(doctorId, appointmentTime);
        return Ok(isAvailable);
    }
}


