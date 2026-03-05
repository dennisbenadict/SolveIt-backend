using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Tournaments.Commands.CancelTournament;

public sealed class CancelTournamentHandler
    : IRequestHandler<CancelTournamentCommand>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelTournamentHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CancelTournamentCommand request,
        CancellationToken cancellationToken)
    {
        var tournament = await _tournamentRepository
            .GetByIdAsync(request.TournamentId, cancellationToken);

        if (tournament is null)
            throw new TournamentNotFoundException();

        if (tournament.OrganizerId != request.OrganizerId)
            throw new TournamentNotOwnedException();

        tournament.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
