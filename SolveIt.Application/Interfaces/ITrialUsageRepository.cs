using SolveIt.Domain.Organizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Interfaces
{
    public interface ITrialUsageRepository
    {
        Task<bool> ExistsByFingerprintAsync(
            string? fingerprint,
            CancellationToken cancellationToken);

        Task<bool> ExistsByIpAsync(
            string? ipAddress,
            CancellationToken cancellationToken);

        Task AddAsync(
            TrialUsage trialUsage,
            CancellationToken cancellationToken);
    }
}
