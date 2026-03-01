using MediatR;

namespace SolveIt.Application.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest;
