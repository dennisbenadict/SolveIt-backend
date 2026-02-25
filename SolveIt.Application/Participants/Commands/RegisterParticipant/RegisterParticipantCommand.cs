using MediatR;

namespace SolveIt.Application.Participants.Commands.RegisterParticipant
{
    public sealed record RegisterParticipantCommand(
        string Name,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword
    ) : IRequest<Guid>;
}
