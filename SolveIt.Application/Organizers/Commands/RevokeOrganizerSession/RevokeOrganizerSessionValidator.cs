using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeOrganizerSession
{
    public sealed class RevokeOrganizerSessionValidator
        : AbstractValidator<RevokeOrganizerSessionCommand>
    {
        public RevokeOrganizerSessionValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
