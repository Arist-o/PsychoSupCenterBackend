
using PsychoSupCenterBackend.Domain.Enums;

namespace PsychoSupCenterBackend.Application.Billing.DTOs;

public sealed record ProcessPaymentDto(
    Guid BillingId,
    PaymentMethod PaymentMethod 
);