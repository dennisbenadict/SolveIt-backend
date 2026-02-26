using MediatR;

namespace SolveIt.Application.Auth.Commands
{
    public sealed record RequestPasswordResetCommand(
        string Email
    ) : IRequest<Unit>;
}
