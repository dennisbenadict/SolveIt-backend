using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeOrganizerSession
{
    public sealed record RevokeOrganizerSessionCommand(string RefreshToken)
        : IRequest<Unit>;
}
