using MediatR;
using SolveIt.Application.Common.DTOs.LeaderboardDTOs;

namespace SolveIt.Application.Tournaments.Queries.GetLeaderboard;

public sealed record GetLeaderboardQuery(Guid TournamentId)
    : IRequest<List<LeaderboardEntryDto>>;
