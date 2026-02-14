using Solvelt.Domain.Submissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvelt.Application.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<bool> ExistsAsync(Guid submissionId, CancellationToken ct);
        Task AddAsync(Submission submission, CancellationToken ct);
    }
}
