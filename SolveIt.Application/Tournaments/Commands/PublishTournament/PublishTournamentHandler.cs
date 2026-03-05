using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Tournaments.Commands.PublishTournament;

public sealed class PublishTournamentHandler
    : IRequestHandler<PublishTournamentCommand>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishTournamentHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        PublishTournamentCommand request,
        CancellationToken cancellationToken)
    {
        var tournament =
            await _tournamentRepository.GetByIdAsync(
                request.TournamentId,
                cancellationToken);

        if (tournament is null)
            throw new TournamentNotFoundException();

        if (tournament.OrganizerId != request.OrganizerId)
            throw new TournamentNotOwnedException();

        tournament.Publish();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}