using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RequestPasswordReset
{
    public sealed record RequestPasswordResetCommand(string Email)
        : IRequest<Unit>;
}
