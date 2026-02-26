using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Auth.Commands
{
    public sealed record ResetPasswordCommand(
        string Token,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<Unit>;
}
