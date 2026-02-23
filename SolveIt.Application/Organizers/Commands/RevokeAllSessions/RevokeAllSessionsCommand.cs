using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeAllSessions
{
    public sealed record RevokeAllSessionsCommand(Guid OrganizerId)
        : IRequest<Unit>;
}
