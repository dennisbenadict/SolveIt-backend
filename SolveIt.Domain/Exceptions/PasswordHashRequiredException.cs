using Solvelt.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class PasswordHashRequiredException
        : DomainException
    {
        public PasswordHashRequiredException()
            : base("Password hash is required.")
        {
        }
    }

}
