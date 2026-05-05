using System;
using System.Collections.Generic;
using System.Text;

namespace PsychoSupCenterBackend.Application.Doctors.DTOs
{
    public sealed record UpdateUnavailabilityBodyDto(
      DateTime StartDate,
      DateTime EndDate,
      string? Reason
  );
}
