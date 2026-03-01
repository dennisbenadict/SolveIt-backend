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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITrialUsageRepository _trialUsageRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public CreateTournamentHandler(
        IOrganizerRepository organizerRepository,
        ITrialUsageRepository trialUsageRepository,
        IUnitOfWork unitOfWork,
        ITournamentRepository tournamentRepository)
    {
        _organizerRepository = organizerRepository;
        _trialUsageRepository = trialUsageRepository;
        _unitOfWork = unitOfWork;
        _tournamentRepository = tournamentRepository;
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

        // Fingerprint check
        var fingerprintUsed =
            await _trialUsageRepository
                .ExistsByFingerprintAsync(
                    request.DeviceFingerprint,
                    cancellationToken);

        if (fingerprintUsed)
            throw new FreeTrialAlreadyUsedException();

        // IP check
        var ipUsed =
            await _trialUsageRepository
                .ExistsByIpAsync(
                    request.IpAddress,
                    cancellationToken);

        if (ipUsed)
            throw new FreeTrialAlreadyUsedException();

        // Activate free trial
        organizer.ActivateFreeTrial();

        // Store usage record
        var trialUsage = TrialUsage.Create(
            organizer.Id,
            request.DeviceFingerprint,
            request.IpAddress);

        await _trialUsageRepository
            .AddAsync(trialUsage, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tournament.Id;
    }
}
