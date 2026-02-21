using Solvelt.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class PasswordResetTokenAlreadyUsedException
        : DomainException
    {
        public PasswordResetTokenAlreadyUsedException()
            : base("Password reset token has already been used.")
        {
        }
    }
}
