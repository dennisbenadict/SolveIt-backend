using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Auth.Commands;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResponseDto>;

