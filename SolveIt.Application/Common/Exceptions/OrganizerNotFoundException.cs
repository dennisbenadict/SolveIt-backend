using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.Exceptions
{
    public sealed class OrganizerNotFoundException
        : DomainException
    {
        public OrganizerNotFoundException()
            : base("Organizer not found.")
        {
        }
    }

}
