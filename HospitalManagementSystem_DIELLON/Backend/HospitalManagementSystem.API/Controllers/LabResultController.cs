using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LabResultController : ControllerBase
{
    private readonly ILabResultService _labResultService;

    public LabResultController(ILabResultService labResultService)
    {
        _labResultService = labResultService;
    }

    [HttpPost]
    public async Task<ActionResult<LabResultDto>> AddLabResult([FromBody] AddLabResultRequest request)
    {
        try
        {
            var labResult = await _labResultService.AddLabResultAsync(request);
            return Ok(labResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<LabResultDto>>> GetAllLabResults()
    {
        var labResults = await _labResultService.GetAllLabResultsAsync();
        return Ok(labResults);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LabResultDto>> GetLabResult(int id)
    {
        var labResult = await _labResultService.GetLabResultByIdAsync(id);
        if (labResult == null) return NotFound();
        return Ok(labResult);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<LabResultDto>>> GetLabResultsByPatient(int patientId)
    {
        var labResults = await _labResultService.GetLabResultsByPatientIdAsync(patientId);
        return Ok(labResults);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LabResultDto>> UpdateLabResult(int id, [FromBody] UpdateLabResultRequest request)
    {
        var labResult = await _labResultService.UpdateLabResultAsync(id, request);
        if (labResult == null) return NotFound();
        return Ok(labResult);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLabResult(int id)
    {
        var result = await _labResultService.DeleteLabResultAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}


