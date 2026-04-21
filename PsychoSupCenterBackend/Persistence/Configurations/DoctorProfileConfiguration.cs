
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasMaxLength(4000);

        builder.Property(x => x.CareerStartDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AverageRating)
            .HasDefaultValue(0.0);

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.ExperienceYears);

        builder.HasMany(x => x.Certificates)
            .WithOne(c => c.DoctorProfile)
            .HasForeignKey(c => c.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Services)
            .WithOne(s => s.DoctorProfile)
            .HasForeignKey(s => s.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Availabilities)
            .WithOne(a => a.DoctorProfile)
            .HasForeignKey(a => a.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Unavailabilities)
            .WithOne(u => u.DoctorProfile)
            .HasForeignKey(u => u.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Specializations)
            .WithOne(s => s.DoctorProfile)
            .HasForeignKey(s => s.DoctorProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Appointments)
            .WithOne(a => a.DoctorProfile)
            .HasForeignKey(a => a.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Reviews)
            .WithOne(r => r.DoctorProfile)
            .HasForeignKey(r => r.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

    
        builder.ToTable("DoctorProfiles");
    }
}