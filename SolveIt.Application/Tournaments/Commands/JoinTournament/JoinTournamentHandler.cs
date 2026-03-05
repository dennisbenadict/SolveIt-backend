using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;
using SolveIt.Domain.TournamentParticipants;

namespace SolveIt.Application.Tournaments.Commands.JoinTournament;

public sealed class JoinTournamentHandler
    : IRequestHandler<JoinTournamentCommand>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentParticipantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public JoinTournamentHandler(
        ITournamentRepository tournamentRepository,
        ITournamentParticipantRepository repository,
        IUnitOfWork unitOfWork)
    {
        _tournamentRepository = tournamentRepository;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        JoinTournamentCommand request,
        CancellationToken cancellationToken)
    {
        var tournament = await _tournamentRepository
            .GetByIdAsync(request.TournamentId, cancellationToken);

        if (tournament is null)
            throw new TournamentNotFoundException();

        if (tournament.Status != Domain.Tournaments.TournamentStatus.Published)
            throw new TournamentNotOpenException();

        var exists = await _repository.ExistsAsync(
            request.TournamentId,
            request.ParticipantId,
            cancellationToken);

        if (exists)
            throw new AlreadyJoinedException();

        var entity = TournamentParticipant.Create(
            request.TournamentId,
            request.ParticipantId);

        await _repository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
