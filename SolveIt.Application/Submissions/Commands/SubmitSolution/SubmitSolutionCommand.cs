using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Submissions.Commands.SubmitSolution
{
    public sealed class SubmitSolutionCommand
    {
        public Guid SubmissionId { get; }
        public Guid UserId { get; }
        public Guid ContestId { get; }
        public Guid ProblemId { get; }

        public SubmitSolutionCommand(
            Guid submissionId,
            Guid userId,
            Guid contestId,
            Guid problemId)
        {
            if (submissionId == Guid.Empty) throw new ArgumentException("SubmissionId is required");
            if (userId == Guid.Empty) throw new ArgumentException("UserId is required");
            if (contestId == Guid.Empty) throw new ArgumentException("ContestId is required");
            if (problemId == Guid.Empty) throw new ArgumentException("ProblemId is required");

            SubmissionId = submissionId;
            UserId = userId;
            ContestId = contestId;
            ProblemId = problemId;
        }
    }
}
