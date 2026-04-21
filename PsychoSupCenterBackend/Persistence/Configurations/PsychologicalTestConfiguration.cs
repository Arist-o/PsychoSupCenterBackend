using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class PsychologicalTestConfiguration : IEntityTypeConfiguration<PsychologicalTest>
{
    public void Configure(EntityTypeBuilder<PsychologicalTest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TestType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ResultJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TakenAt).IsRequired();

        builder.ToTable("PsychologicalTests");
    }
}