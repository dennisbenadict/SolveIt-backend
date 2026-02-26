using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Auth.Commands;

public sealed record AuthenticateUserCommand(
    string Identifier,
    string Password
) : IRequest<AuthResponseDto>;

