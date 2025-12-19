using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Data;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Services;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;

    public PatientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientResponse> AddPatientAsync(AddPatientRequest request)
    {
        var patient = new Patient
        {
            Name = request.Name,
            Age = request.Age,
            Insurance = request.Insurance,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            DoctorId = request.DoctorId
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return await GetPatientByIdAsync(patient.PatientId) ?? 
            throw new InvalidOperationException("Failed to retrieve created patient");
    }

    public async Task<PatientResponse?> GetPatientByIdAsync(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Doctor)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient == null) return null;

        return new PatientResponse
        {
            PatientId = patient.PatientId,
            Name = patient.Name,
            Age = patient.Age,
            Insurance = patient.Insurance,
            Address = patient.Address,
            Phone = patient.Phone,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth,
            DoctorId = patient.DoctorId,
            DoctorName = patient.Doctor?.Name
        };
    }

    public async Task<List<PatientResponse>> GetAllPatientsAsync()
    {
        var patients = await _context.Patients
            .Include(p => p.Doctor)
            .ToListAsync();

        return patients.Select(p => new PatientResponse
        {
            PatientId = p.PatientId,
            Name = p.Name,
            Age = p.Age,
            Insurance = p.Insurance,
            Address = p.Address,
            Phone = p.Phone,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            DoctorId = p.DoctorId,
            DoctorName = p.Doctor?.Name
        }).ToList();
    }

    public async Task<PatientResponse?> UpdatePatientAsync(int id, UpdatePatientRequest request)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return null;

        if (request.Name != null) patient.Name = request.Name;
        if (request.Age.HasValue) patient.Age = request.Age.Value;
        if (request.Insurance != null) patient.Insurance = request.Insurance;
        if (request.Address != null) patient.Address = request.Address;
        if (request.Phone != null) patient.Phone = request.Phone;
        if (request.Email != null) patient.Email = request.Email;
        if (request.DateOfBirth.HasValue) patient.DateOfBirth = request.DateOfBirth;
        if (request.DoctorId.HasValue) patient.DoctorId = request.DoctorId;

        await _context.SaveChangesAsync();
        return await GetPatientByIdAsync(id);
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
            .Include(p => p.Invoices)
                .ThenInclude(i => i.Payments)
            .Include(p => p.LabResults)
            .Include(p => p.Prescriptions)
            .FirstOrDefaultAsync(p => p.PatientId == id);
        
        if (patient == null) return false;

        // Delete all related appointments
        _context.Appointments.RemoveRange(patient.Appointments);

        // Delete all payments for patient's invoices first
        foreach (var invoice in patient.Invoices)
        {
            _context.Payments.RemoveRange(invoice.Payments);
        }

        // Delete all invoices
        _context.Invoices.RemoveRange(patient.Invoices);

        // Delete all lab results
        _context.LabResults.RemoveRange(patient.LabResults);

        // Delete all prescriptions
        _context.Prescriptions.RemoveRange(patient.Prescriptions);

        // MedicalRecord will be deleted automatically due to Cascade delete behavior

        // Now delete the patient
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
}


