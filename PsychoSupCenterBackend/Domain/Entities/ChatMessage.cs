using PsychoSupCenterBackend.Domain.Common;
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ChatRoom ChatRoom { get; set; } = null!;
    public ApplicationUser Sender { get; set; } = null!;
}