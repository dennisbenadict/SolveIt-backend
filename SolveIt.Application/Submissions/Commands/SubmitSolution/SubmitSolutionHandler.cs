using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Common;
using SolveIt.Domain.Submissions;

namespace SolveIt.Application.Submissions.Commands.SubmitSolution;

public sealed class SubmitSolutionHandler
    : IRequestHandler<SubmitSolutionCommand, Guid>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ITournamentParticipantRepository _participantRepository;
    private readonly ISubmissionQueuePublisher _queuePublisher;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitSolutionHandler(
        ISubmissionRepository submissionRepository,
        ITournamentParticipantRepository participantRepository,
        ISubmissionQueuePublisher queuePublisher,
        IUnitOfWork unitOfWork)
    {
        _submissionRepository = submissionRepository;
        _participantRepository = participantRepository;
        _queuePublisher = queuePublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        SubmitSolutionCommand request,
        CancellationToken cancellationToken)
    {
        // Validate participant joined tournament
        var joined = await _participantRepository.ExistsAsync(
            request.TournamentId,
            request.ParticipantId,
            cancellationToken);

        if (!joined)
            throw new InvalidOperationException(
                "Participant has not joined this tournament.");

        // Convert language string → enum
        if (!Enum.TryParse<ProgrammingLanguage>(
                request.Language,
                true,
                out var language))
        {
            throw new ArgumentException("Unsupported programming language.");
        }

        // Create submission aggregate
        var submission = Submission.Create(
            request.ProblemId,
            request.ParticipantId,
            language,
            request.SourceCode);

        // Persist submission
        await _submissionRepository.AddAsync(
            submission,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish execution job
        await _queuePublisher.PublishAsync(
            submission.Id,
            submission.TournamentProblemId,
            cancellationToken);

        return submission.Id;
    }
}
