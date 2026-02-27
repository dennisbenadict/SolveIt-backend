using MediatR;

namespace SolveIt.Application.Auth.Commands.RequestPasswordReset
{
    public sealed record RequestPasswordResetCommand(
        string Email
    ) : IRequest<Unit>;
}
