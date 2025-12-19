using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services;

namespace HospitalManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalRecordController : ControllerBase
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    [HttpPost]
    public async Task<ActionResult<MedicalRecordDto>> AddMedicalRecord([FromBody] AddMedicalRecordRequest request)
    {
        try
        {
            var record = await _medicalRecordService.AddMedicalRecordAsync(request);
            return Ok(record);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecordByPatient(int patientId)
    {
        var record = await _medicalRecordService.GetMedicalRecordByPatientIdAsync(patientId);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int id)
    {
        var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(int id, [FromBody] UpdateMedicalRecordRequest request)
    {
        var record = await _medicalRecordService.UpdateMedicalRecordAsync(id, request);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPost("clinical-entry")]
    public async Task<ActionResult<ClinicalEntryDto>> AddClinicalEntry([FromBody] AddClinicalEntryRequest request)
    {
        var entry = await _medicalRecordService.AddClinicalEntryAsync(request);
        return Ok(entry);
    }

    [HttpGet("{recordId}/clinical-entries")]
    public async Task<ActionResult<List<ClinicalEntryDto>>> GetClinicalEntries(int recordId)
    {
        var entries = await _medicalRecordService.GetClinicalEntriesByRecordIdAsync(recordId);
        return Ok(entries);
    }
}

