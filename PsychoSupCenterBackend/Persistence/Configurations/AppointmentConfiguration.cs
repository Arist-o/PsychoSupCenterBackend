
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.ScheduledAt)
            .IsRequired();

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.HasOne(x => x.Billing)
            .WithOne(b => b.Appointment)
            .HasForeignKey<Appointment>(x => x.BillingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.ChatRoom)
            .WithOne(c => c.Appointment)
            .HasForeignKey<Appointment>(x => x.ChatRoomId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.PsychologicalTest)
            .WithOne(t => t.Appointment)
            .HasForeignKey<PsychologicalTest>(t => t.AppointmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Review)
            .WithOne(r => r.Appointment)
            .HasForeignKey<Review>(r => r.AppointmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


        builder.ToTable("Appointments");
    }
}