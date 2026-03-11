using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Tournaments;

namespace SolveIt.Application.Tournaments.Commands.CreateTournament;

public sealed class CreateTournamentHandler
    : IRequestHandler<CreateTournamentCommand, Guid>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly ITrialUsageRepository _trialUsageRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTournamentHandler(
        IOrganizerRepository organizerRepository,
        ITrialUsageRepository trialUsageRepository,
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _trialUsageRepository = trialUsageRepository;
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateTournamentCommand request,
        CancellationToken cancellationToken)
    {
        var organizer = await _organizerRepository
            .GetByIdAsync(request.OrganizerId, cancellationToken);

        if (organizer is null)
            throw new OrganizerNotFoundException();

        if (organizer.HasUsedFreeTrial)
            throw new FreeTrialAlreadyUsedException();

        // Device fingerprint protection
        var fingerprintUsed =
            await _trialUsageRepository
                .ExistsByFingerprintAsync(
                    request.DeviceFingerprint,
                    cancellationToken);

        if (fingerprintUsed)
            throw new FreeTrialAlreadyUsedException();

        // IP protection
        var ipUsed =
            await _trialUsageRepository
                .ExistsByIpAsync(
                    request.IpAddress,
                    cancellationToken);

        if (ipUsed)
            throw new FreeTrialAlreadyUsedException();

        // Create tournament
        var tournament = Tournament.Create(
            organizer.Id,
            request.Title,
            request.Description,
            request.StartTimeUtc,
            request.EndTimeUtc,
            true);

        await _tournamentRepository.AddAsync(
            tournament,
            cancellationToken);

        // Mark trial consumed
        organizer.MarkFreeTrialUsed();

        // Record trial usage
        if (string.IsNullOrWhiteSpace(request.DeviceFingerprint))
            throw new ArgumentException("Device fingerprint is required.");

        if (string.IsNullOrWhiteSpace(request.IpAddress))
            throw new ArgumentException("IP address is required.");

        var trialUsage = TrialUsage.Create(
            organizer.Id,
            request.DeviceFingerprint,
            request.IpAddress);

        await _trialUsageRepository.AddAsync(
            trialUsage,
            cancellationToken);

        // Single transaction save
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tournament.Id;
    }
}