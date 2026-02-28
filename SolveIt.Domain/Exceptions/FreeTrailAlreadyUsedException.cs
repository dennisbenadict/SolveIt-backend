using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Domain.Exceptions
{
    public sealed class FreeTrialAlreadyUsedException 
        : DomainException
    {
        public FreeTrialAlreadyUsedException()
            : base("The free trial has already been used for this account.")
        {
        }
    }
}
