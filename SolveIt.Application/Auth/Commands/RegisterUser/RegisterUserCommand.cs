using MediatR;
using SolveIt.Domain.Common;

namespace SolveIt.Application.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword
) : IRequest<Guid>;

