using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DoctorAvailabilities.DTOs
{
    public sealed record AddDoctorUnavailabilityDto(Guid DoctorProfileId, DateTime StartDate, DateTime EndDate, string? Reason);
}
