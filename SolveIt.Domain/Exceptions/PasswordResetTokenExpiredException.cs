using SolveIt.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class PasswordResetTokenExpiredException
        : DomainException
    {
        public PasswordResetTokenExpiredException()
            : base("Password reset token has expired.")
        {
        }
    }
}
