using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Interfaces
{
    public interface IContestRepository
    {
        Task<bool> IsContestActiveAsync(Guid contestId, CancellationToken ct);
    }
}
