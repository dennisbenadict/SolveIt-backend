using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.PasswordResetDTOs
{
    public sealed record ForgotPasswordRequest(
        string Email);
}
