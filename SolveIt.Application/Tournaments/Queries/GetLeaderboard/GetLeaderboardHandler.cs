using MediatR;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Common.DTOs.LeaderboardDTOs;
using SolveIt.Domain.Submissions;
using SolveIt.Application.Interfaces;

namespace SolveIt.Application.Tournaments.Queries.GetLeaderboard;

public sealed class GetLeaderboardHandler
    : IRequestHandler<GetLeaderboardQuery, List<LeaderboardEntryDto>>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GetLeaderboardHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<List<LeaderboardEntryDto>> Handle(
        GetLeaderboardQuery request,
        CancellationToken cancellationToken)
    {
        var submissions =
            await _submissionRepository.GetAcceptedByTournamentAsync(
                request.TournamentId,
                cancellationToken);

        var grouped = submissions
            .GroupBy(s => s.ParticipantId)
            .Select(g => new
            {
                ParticipantId = g.Key,
                Solved = g.Select(x => x.TournamentProblemId).Distinct().Count(),
                LastAccepted = g.Max(x => x.CreatedAtUtc)
            })
            .OrderByDescending(x => x.Solved)
            .ThenBy(x => x.LastAccepted)
            .ToList();

        int rank = 1;

        return grouped.Select(x => new LeaderboardEntryDto
        {
            ParticipantId = x.ParticipantId,
            SolvedProblems = x.Solved,
            LastAcceptedAtUtc = x.LastAccepted,
            Rank = rank++
        }).ToList();
    }
}
