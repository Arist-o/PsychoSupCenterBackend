namespace PsychoSupCenterBackend.Application.Admin.DTOs;

public sealed record DashboardStatsDto(
    int TotalPatients,
    int TotalDoctors,
    int TotalAppointments,
    decimal TotalRevenue,
    List<MonthlyStatDto> AppointmentsPerMonth
);

public sealed record MonthlyStatDto(string Month, int Count);
