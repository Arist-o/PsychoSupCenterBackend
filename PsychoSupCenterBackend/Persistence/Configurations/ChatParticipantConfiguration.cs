using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsychoSupCenterBackend.Domain.Entities;

namespace PsychoSupCenterBackend.Persistence.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LastReadAt)
            .IsRequired();

    
        builder.ToTable("ChatParticipants");
    }
}