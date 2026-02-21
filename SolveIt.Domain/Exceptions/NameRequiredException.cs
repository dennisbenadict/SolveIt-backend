using Solvelt.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class NameRequiredException
        : DomainException
    {
        public NameRequiredException()
            : base("Name is required.")
        {
        }
    }

}
