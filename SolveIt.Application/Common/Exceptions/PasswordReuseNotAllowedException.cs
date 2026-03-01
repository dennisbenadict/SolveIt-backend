using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.Exceptions
{
    public sealed class PasswordReuseNotAllowedException : DomainException
    {
        public PasswordReuseNotAllowedException()
            : base("New password must be different from current password.")
        {
        }
    }
}
