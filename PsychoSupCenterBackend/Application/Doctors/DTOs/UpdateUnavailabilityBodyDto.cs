using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Doctors.DTOs
{
    public sealed record UpdateUnavailabilityBodyDto(
      DateTime StartDate,
      DateTime EndDate,
      string? Reason
  );
}
