
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class DoctorServiceConfiguration : IEntityTypeConfiguration<DoctorService>
{
    public void Configure(EntityTypeBuilder<DoctorService> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(x => x.Appointments)
            .WithOne(a => a.DoctorService)
            .HasForeignKey(a => a.DoctorServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Billings)
            .WithOne(b => b.DoctorService)
            .HasForeignKey(b => b.DoctorServiceId)
            .OnDelete(DeleteBehavior.Restrict);

    
        builder.ToTable("DoctorServices");
    }
}