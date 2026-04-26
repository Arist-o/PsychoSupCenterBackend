using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DoctorSpecializations.DTOs
{
    public sealed record CreateSpecializationDto(Guid DoctorProfileId, string Name);
}
