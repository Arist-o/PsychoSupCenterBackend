
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Billing.DTOs;

public sealed record BillingResponseDto(
    Guid Id,
    Guid DoctorServiceId,
    string ServiceName,
    decimal Amount,
    PaymentStatus PaymentStatus,
    DateTime CreatedAt,
    DateTime? PaidAt
);