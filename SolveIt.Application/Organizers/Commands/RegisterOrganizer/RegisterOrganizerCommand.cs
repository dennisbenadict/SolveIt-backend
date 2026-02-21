using MediatR;

namespace SolveIt.Application.Organizers.Commands.RegisterOrganizer;

public sealed record RegisterOrganizerCommand(
    string Name,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword
) : IRequest<Guid>;

