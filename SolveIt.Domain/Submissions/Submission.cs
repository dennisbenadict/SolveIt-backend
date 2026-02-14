using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solvelt.Domain.Submissions
{
//What is a GUID (instead of int)?

//GUID = Globally Unique Identifier
//It is a 128-bit value designed to be unique across space and time.

//Meaning:
//Unique across users
//Unique across servers
//Unique across regions
//Unique even without a database
//No coordination required.
    public sealed class Submission
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid ContestId { get; private set; }
        public Guid ProblemId { get; private set; }
        public SubmissionStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private Submission() { } 

        public Submission(
            Guid id,
            Guid userId,
            Guid contestId,
            Guid problemId)
        {
            if (id == Guid.Empty) throw new ArgumentException("SubmissionId is required");
            if (userId == Guid.Empty) throw new ArgumentException("UserId is required");
            if (contestId == Guid.Empty) throw new ArgumentException("ContestId is required");
            if (problemId == Guid.Empty) throw new ArgumentException("ProblemId is required");

            Id = id;
            UserId = userId;
            ContestId = contestId;
            ProblemId = problemId;

            Status = SubmissionStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void MarkQueued()
        {
            EnsureState(SubmissionStatus.Pending);
            Status = SubmissionStatus.Queued;
        }

        public void MarkRunning()
        {
            EnsureState(SubmissionStatus.Queued);
            Status = SubmissionStatus.Running;
        }

        public void MarkSucceeded()
        {
            EnsureState(SubmissionStatus.Running);
            Status = SubmissionStatus.Succeeded;
        }

        public void MarkFailed()
        {
            EnsureState(SubmissionStatus.Running);
            Status = SubmissionStatus.Failed;
        }

        public void MarkTimedOut()
        {
            EnsureState(SubmissionStatus.Running);
            Status = SubmissionStatus.TimedOut;
        }

        public void Reject()
        {
            EnsureState(SubmissionStatus.Pending);
            Status = SubmissionStatus.Rejected;
        }

        private void EnsureState(SubmissionStatus expected)
        {
            if (Status != expected)
                throw new InvalidOperationException(
                    $"Invalid state transition: {Status} → {expected}");
        }
    }

}
