using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Infrastructure;

namespace HospitalManagementSystem.API.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly ApplicationDbContext _context;
    private readonly ISimClock _simClock;

    public MedicalRecordService(ApplicationDbContext context, ISimClock simClock)
    {
        _context = context;
        _simClock = simClock;
    }

    public async Task<MedicalRecordDto> AddMedicalRecordAsync(AddMedicalRecordRequest request)
    {
        // Check if patient already has a medical record
        var existingRecord = await _context.MedicalRecords
            .FirstOrDefaultAsync(mr => mr.PatientId == request.PatientId);

        if (existingRecord != null)
        {
            throw new InvalidOperationException("Patient already has a medical record");
        }

        var medicalRecord = new MedicalRecord
        {
            PatientId = request.PatientId,
            Diagnosis = request.Diagnosis,
            Treatment = request.Treatment,
            CreatedAt = _simClock.GetCurrentTime()
        };

        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();

        return await GetMedicalRecordByIdAsync(medicalRecord.RecordId) ?? 
            throw new InvalidOperationException("Failed to retrieve created medical record");
    }

    public async Task<MedicalRecordDto?> GetMedicalRecordByPatientIdAsync(int patientId)
    {
        var medicalRecord = await _context.MedicalRecords
            .Include(mr => mr.Patient)
            .Include(mr => mr.ClinicalEntries)
            .FirstOrDefaultAsync(mr => mr.PatientId == patientId);

        if (medicalRecord == null) return null;

        return new MedicalRecordDto
        {
            RecordId = medicalRecord.RecordId,
            CreatedAt = medicalRecord.CreatedAt,
            Diagnosis = medicalRecord.Diagnosis,
            Treatment = medicalRecord.Treatment,
            PatientId = medicalRecord.PatientId,
            PatientName = medicalRecord.Patient.Name,
            ClinicalEntries = medicalRecord.ClinicalEntries.Select(ce => new ClinicalEntryDto
            {
                EntryId = ce.EntryId,
                Date = ce.Date,
                Notes = ce.Notes,
                Diagnosis = ce.Diagnosis
            }).ToList()
        };
    }

    public async Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int recordId)
    {
        var medicalRecord = await _context.MedicalRecords
            .Include(mr => mr.Patient)
            .Include(mr => mr.ClinicalEntries)
            .FirstOrDefaultAsync(mr => mr.RecordId == recordId);

        if (medicalRecord == null) return null;

        return new MedicalRecordDto
        {
            RecordId = medicalRecord.RecordId,
            CreatedAt = medicalRecord.CreatedAt,
            Diagnosis = medicalRecord.Diagnosis,
            Treatment = medicalRecord.Treatment,
            PatientId = medicalRecord.PatientId,
            PatientName = medicalRecord.Patient.Name,
            ClinicalEntries = medicalRecord.ClinicalEntries.Select(ce => new ClinicalEntryDto
            {
                EntryId = ce.EntryId,
                Date = ce.Date,
                Notes = ce.Notes,
                Diagnosis = ce.Diagnosis
            }).ToList()
        };
    }

    public async Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int recordId, UpdateMedicalRecordRequest request)
    {
        var medicalRecord = await _context.MedicalRecords.FindAsync(recordId);
        if (medicalRecord == null) return null;

        if (request.Diagnosis != null) medicalRecord.Diagnosis = request.Diagnosis;
        if (request.Treatment != null) medicalRecord.Treatment = request.Treatment;

        await _context.SaveChangesAsync();
        return await GetMedicalRecordByIdAsync(recordId);
    }

    public async Task<ClinicalEntryDto> AddClinicalEntryAsync(AddClinicalEntryRequest request)
    {
        var clinicalEntry = new ClinicalEntry
        {
            RecordId = request.RecordId,
            Notes = request.Notes,
            Diagnosis = request.Diagnosis,
            Date = _simClock.GetCurrentTime()
        };

        _context.ClinicalEntries.Add(clinicalEntry);
        await _context.SaveChangesAsync();

        return new ClinicalEntryDto
        {
            EntryId = clinicalEntry.EntryId,
            Date = clinicalEntry.Date,
            Notes = clinicalEntry.Notes,
            Diagnosis = clinicalEntry.Diagnosis
        };
    }

    public async Task<List<ClinicalEntryDto>> GetClinicalEntriesByRecordIdAsync(int recordId)
    {
        var entries = await _context.ClinicalEntries
            .Where(ce => ce.RecordId == recordId)
            .OrderByDescending(ce => ce.Date)
            .ToListAsync();

        return entries.Select(ce => new ClinicalEntryDto
        {
            EntryId = ce.EntryId,
            Date = ce.Date,
            Notes = ce.Notes,
            Diagnosis = ce.Diagnosis
        }).ToList();
    }
}


