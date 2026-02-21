using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Organizers.Commands.RefreshOrganizerToken;

public sealed record RefreshOrganizerTokenCommand(string RefreshToken)
    : IRequest<AuthResponseDto>;

