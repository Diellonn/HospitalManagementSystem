using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.API.Models;

namespace HospitalManagementSystem.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<ClinicalEntry> ClinicalEntries { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<LabResult> LabResults { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName).IsRequired();
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Insurance).IsRequired();
            
            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Doctor configuration
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Specialization).IsRequired();
            
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Doctors)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Nurse configuration
        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.HasKey(e => e.NurseId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Ward).IsRequired();
            
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Nurses)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Reason).IsRequired();
            
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MedicalRecord configuration
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);
            
            entity.HasOne(e => e.Patient)
                .WithOne(p => p.MedicalRecord)
                .HasForeignKey<MedicalRecord>(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ClinicalEntry configuration
        modelBuilder.Entity<ClinicalEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId);
            entity.Property(e => e.Notes).IsRequired();
            
            entity.HasOne(e => e.MedicalRecord)
                .WithMany(mr => mr.ClinicalEntries)
                .HasForeignKey(e => e.RecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired();
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentMethod).IsRequired();
            
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // LabResult configuration
        modelBuilder.Entity<LabResult>(entity =>
        {
            entity.HasKey(e => e.ResultId);
            entity.Property(e => e.Type).IsRequired();
            
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.LabResults)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.LabResults)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Nurse)
                .WithMany(n => n.LabResults)
                .HasForeignKey(e => e.NurseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Prescription configuration
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId);
            entity.Property(e => e.Instructions).IsRequired();
            
            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId);
            entity.Property(e => e.Name).IsRequired();
        });

        // Room configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);
            entity.Property(e => e.Type).IsRequired();
            
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Rooms)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}


