using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
{
    public void Configure(EntityTypeBuilder<PatientProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.MilitaryId)
            .HasMaxLength(50);

        builder.Property(x => x.EmergencyContact)
            .HasMaxLength(200);

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("IX_PatientProfile_UserId");

        builder.HasMany(x => x.Appointments)
            .WithOne(a => a.PatientProfile)
            .HasForeignKey(a => a.PatientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Reviews)
            .WithOne(r => r.PatientProfile)
            .HasForeignKey(r => r.PatientProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("PatientProfiles");
    }
}