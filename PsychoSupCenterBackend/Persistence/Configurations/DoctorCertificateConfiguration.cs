using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class DoctorCertificateConfiguration : IEntityTypeConfiguration<DoctorCertificate>
{
    public void Configure(EntityTypeBuilder<DoctorCertificate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CertificateUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.AddedAt).IsRequired();

        builder.ToTable("DoctorCertificates");
    }
}