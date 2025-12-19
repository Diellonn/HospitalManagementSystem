using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> AddPatient([FromBody] AddPatientRequest request)
    {
        try
        {
            var patient = await _patientService.AddPatientAsync(request);
            return Ok(patient);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<PatientResponse>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientResponse>> GetPatient(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientResponse>> UpdatePatient(int id, [FromBody] UpdatePatientRequest request)
    {
        var patient = await _patientService.UpdatePatientAsync(id, request);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var result = await _patientService.DeletePatientAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}


