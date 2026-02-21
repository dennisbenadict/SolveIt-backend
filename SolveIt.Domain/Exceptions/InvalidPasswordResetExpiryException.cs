using Solvelt.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class InvalidPasswordResetExpiryException
        : DomainException
    {
        public InvalidPasswordResetExpiryException()
            : base("Password reset token expiry must be in the future.")
        {
        }
    }
}
