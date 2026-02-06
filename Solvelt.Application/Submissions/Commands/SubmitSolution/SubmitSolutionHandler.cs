using Solvelt.Application.Submissions.Interfaces;
using Solvelt.Domain.Submissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvelt.Application.Submissions.Commands.SubmitSolution
{
    public sealed class SubmitSolutionHandler
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IContestRepository _contestRepository;
        private readonly ISubmissionQueuePublisher _queuePublisher;

        public SubmitSolutionHandler(
            ISubmissionRepository submissionRepository,
            IContestRepository contestRepository,
            ISubmissionQueuePublisher queuePublisher)
        {
            _submissionRepository = submissionRepository;
            _contestRepository = contestRepository;
            _queuePublisher = queuePublisher;
        }

        public async Task HandleAsync(
            SubmitSolutionCommand command,
            CancellationToken ct)
        {
            // 1. Idempotency check
            if (await _submissionRepository.ExistsAsync(command.SubmissionId, ct))
                return; // safe retry, do nothing

            // 2. Contest validation
            var isActive = await _contestRepository.IsContestActiveAsync(command.ContestId, ct);
            if (!isActive)
                throw new InvalidOperationException("Contest is not active");

            // 3. Create domain entity
            var submission = new Submission(
                command.SubmissionId,
                command.UserId,
                command.ContestId,
                command.ProblemId);

            // 4. Persist
            await _submissionRepository.AddAsync(submission, ct);

            // 5. Publish execution job
            await _queuePublisher.PublishAsync(
                submission.Id,
                submission.ContestId,
                ct);
        }
    }
}
