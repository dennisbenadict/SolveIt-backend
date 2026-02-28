using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Organizers;
using SolveIt.Infrastructure.Persistence;

namespace SolveIt.Infrastructure.Repositories;

public sealed class TrialUsageRepository
    : ITrialUsageRepository
{
    private readonly SolveItDbContext _context;

    public TrialUsageRepository(SolveItDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByFingerprintAsync(
        string? fingerprint,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fingerprint))
            return false;

        return await _context.TrialUsages
            .AnyAsync(t => t.DeviceFingerprint == fingerprint, cancellationToken);
    }

    public async Task<bool> ExistsByIpAsync(
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        return await _context.TrialUsages
            .AnyAsync(t => t.IpAddress == ipAddress, cancellationToken);
    }

    public async Task AddAsync(
        TrialUsage trialUsage,
        CancellationToken cancellationToken)
    {
        await _context.TrialUsages
            .AddAsync(trialUsage, cancellationToken);
    }
}