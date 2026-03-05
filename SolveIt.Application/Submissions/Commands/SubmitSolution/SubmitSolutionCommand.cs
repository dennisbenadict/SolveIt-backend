using MediatR;

namespace SolveIt.Application.Submissions.Commands.SubmitSolution;

public sealed record SubmitSolutionCommand(
    Guid TournamentId,
    Guid ProblemId,
    Guid ParticipantId,
    string Language,
    string SourceCode
) : IRequest<Guid>;