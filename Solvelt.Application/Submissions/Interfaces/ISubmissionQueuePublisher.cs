using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvelt.Application.Submissions.Interfaces
{
    public interface ISubmissionQueuePublisher
    {
        Task PublishAsync(Guid submissionId, Guid contestId, CancellationToken ct);
    }
}
