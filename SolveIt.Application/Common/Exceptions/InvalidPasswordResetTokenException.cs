using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.Exceptions
{
    public sealed class InvalidPasswordResetTokenException
        : DomainException
    {
        public InvalidPasswordResetTokenException()
            : base("Invalid password reset token.")
        {
        }
    }

}
