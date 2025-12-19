using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Infrastructure;

namespace HospitalManagementSystem.API.Services;

public class LabResultService : ILabResultService
{
    private readonly ApplicationDbContext _context;
    private readonly ISimClock _simClock;
    private readonly IEmailService _emailService;

    public LabResultService(ApplicationDbContext context, ISimClock simClock, IEmailService emailService)
    {
        _context = context;
        _simClock = simClock;
        _emailService = emailService;
    }

    public async Task<LabResultDto> AddLabResultAsync(AddLabResultRequest request)
    {
        var labResult = new LabResult
        {
            PatientId = request.PatientId,
            Type = request.Type,
            ResultData = request.ResultData,
            TestDate = request.TestDate,
            DoctorId = request.DoctorId,
            NurseId = request.NurseId
        };

        _context.LabResults.Add(labResult);
        await _context.SaveChangesAsync();

        // Send notification email
        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient != null && !string.IsNullOrEmpty(patient.Email))
        {
            await _emailService.SendLabResultNotificationAsync(
                patient.Email,
                patient.Name,
                request.Type
            );
        }

        return await GetLabResultByIdAsync(labResult.ResultId) ?? 
            throw new InvalidOperationException("Failed to retrieve created lab result");
    }

    public async Task<LabResultDto?> GetLabResultByIdAsync(int id)
    {
        var labResult = await _context.LabResults
            .Include(lr => lr.Patient)
            .Include(lr => lr.Doctor)
            .Include(lr => lr.Nurse)
            .FirstOrDefaultAsync(lr => lr.ResultId == id);

        if (labResult == null) return null;

        return new LabResultDto
        {
            ResultId = labResult.ResultId,
            Type = labResult.Type,
            ResultData = labResult.ResultData,
            TestDate = labResult.TestDate,
            ResultDate = labResult.ResultDate,
            Diagnosis = labResult.Diagnosis,
            Treatment = labResult.Treatment,
            PatientId = labResult.PatientId,
            PatientName = labResult.Patient.Name,
            DoctorId = labResult.DoctorId,
            DoctorName = labResult.Doctor?.Name,
            NurseId = labResult.NurseId,
            NurseName = labResult.Nurse?.Name
        };
    }

    public async Task<List<LabResultDto>> GetAllLabResultsAsync()
    {
        var labResults = await _context.LabResults
            .Include(lr => lr.Patient)
            .Include(lr => lr.Doctor)
            .Include(lr => lr.Nurse)
            .ToListAsync();

        return labResults.Select(lr => new LabResultDto
        {
            ResultId = lr.ResultId,
            Type = lr.Type,
            ResultData = lr.ResultData,
            TestDate = lr.TestDate,
            ResultDate = lr.ResultDate,
            Diagnosis = lr.Diagnosis,
            Treatment = lr.Treatment,
            PatientId = lr.PatientId,
            PatientName = lr.Patient.Name,
            DoctorId = lr.DoctorId,
            DoctorName = lr.Doctor?.Name,
            NurseId = lr.NurseId,
            NurseName = lr.Nurse?.Name
        }).ToList();
    }

    public async Task<List<LabResultDto>> GetLabResultsByPatientIdAsync(int patientId)
    {
        var labResults = await _context.LabResults
            .Include(lr => lr.Patient)
            .Include(lr => lr.Doctor)
            .Include(lr => lr.Nurse)
            .Where(lr => lr.PatientId == patientId)
            .ToListAsync();

        return labResults.Select(lr => new LabResultDto
        {
            ResultId = lr.ResultId,
            Type = lr.Type,
            ResultData = lr.ResultData,
            TestDate = lr.TestDate,
            ResultDate = lr.ResultDate,
            Diagnosis = lr.Diagnosis,
            Treatment = lr.Treatment,
            PatientId = lr.PatientId,
            PatientName = lr.Patient.Name,
            DoctorId = lr.DoctorId,
            DoctorName = lr.Doctor?.Name,
            NurseId = lr.NurseId,
            NurseName = lr.Nurse?.Name
        }).ToList();
    }

    public async Task<LabResultDto?> UpdateLabResultAsync(int id, UpdateLabResultRequest request)
    {
        var labResult = await _context.LabResults.FindAsync(id);
        if (labResult == null) return null;

        if (request.ResultData != null) labResult.ResultData = request.ResultData;
        if (request.ResultDate.HasValue) labResult.ResultDate = request.ResultDate;
        if (request.Diagnosis != null) labResult.Diagnosis = request.Diagnosis;
        if (request.Treatment != null) labResult.Treatment = request.Treatment;

        await _context.SaveChangesAsync();
        return await GetLabResultByIdAsync(id);
    }

    public async Task<bool> DeleteLabResultAsync(int id)
    {
        var labResult = await _context.LabResults.FindAsync(id);
        if (labResult == null) return false;

        _context.LabResults.Remove(labResult);
        await _context.SaveChangesAsync();
        return true;
    }
}


