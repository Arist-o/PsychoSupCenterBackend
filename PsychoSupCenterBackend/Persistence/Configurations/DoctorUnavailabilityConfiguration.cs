
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class DoctorUnavailabilityConfiguration : IEntityTypeConfiguration<DoctorUnavailability>
{
    public void Configure(EntityTypeBuilder<DoctorUnavailability> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.ToTable("DoctorUnavailabilities");
    }
}