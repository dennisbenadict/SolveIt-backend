using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResponseDto>;

