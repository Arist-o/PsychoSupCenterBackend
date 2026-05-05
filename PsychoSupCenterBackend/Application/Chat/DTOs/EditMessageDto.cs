using PsychoSupCenterBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PsychoSupCenterBackend.Application.Chat.DTOs
{
    public sealed record EditMessageDto(Guid EditorUserId, string NewContent);
}
