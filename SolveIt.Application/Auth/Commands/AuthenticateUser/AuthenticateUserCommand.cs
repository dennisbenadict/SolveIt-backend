using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Auth.Commands.AuthenticateUser;

public sealed record AuthenticateUserCommand(
    string Identifier,
    string Password
) : IRequest<AuthResponseDto>;

