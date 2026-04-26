using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.DTOs
{
    public sealed record LogoutDto(string RefreshToken);
}
