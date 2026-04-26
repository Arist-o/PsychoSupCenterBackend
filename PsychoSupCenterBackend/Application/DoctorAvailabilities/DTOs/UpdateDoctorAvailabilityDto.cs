using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DoctorAvailabilities.DTOs
{
    public sealed record UpdateDoctorAvailabilityDto(DayOfWeek Day, TimeSpan StartTime, TimeSpan EndTime);

}
