using PsychoSupCenterBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Billing.DTOs
{
    public sealed record UpdatePaymentStatusDto(PaymentStatus NewStatus);
}
