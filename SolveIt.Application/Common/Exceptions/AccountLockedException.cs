using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.Exceptions
{
    public sealed class AccountLockedException : DomainException
    {
        public AccountLockedException()
            : base("Account is temporarily locked due to multiple failed login attempts.")
        {
        }
    }
}
