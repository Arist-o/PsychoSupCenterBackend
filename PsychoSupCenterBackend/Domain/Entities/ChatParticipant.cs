using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class ChatParticipant : BaseEntity
{
    public Guid ChatRoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LastReadAt { get; set; } = DateTime.UtcNow;

    public ChatRoom ChatRoom { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}