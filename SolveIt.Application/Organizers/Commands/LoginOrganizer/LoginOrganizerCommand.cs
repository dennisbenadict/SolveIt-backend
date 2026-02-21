using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace SolveIt.Application.Organizers.Commands.LoginOrganizer;

public sealed record LoginOrganizerCommand(
    string Identifier,
    string Password
) : IRequest<AuthResponseDto>;

