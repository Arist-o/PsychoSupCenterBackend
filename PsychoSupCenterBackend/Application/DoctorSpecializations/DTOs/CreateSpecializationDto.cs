using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DoctorSpecializations.DTOs
{
    public sealed record CreateSpecializationDto(string Name,string Description);
}
