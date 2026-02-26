using MediatR;
using SolveIt.Domain.Common;

namespace SolveIt.Application.Auth.Commands;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    UserRole Role
) : IRequest<Guid>;

