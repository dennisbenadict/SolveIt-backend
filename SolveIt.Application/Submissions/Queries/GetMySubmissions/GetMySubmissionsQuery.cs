using MediatR;
using SolveIt.Application.Common.DTOs;
using SolveIt.Application.Common.DTOs.FinalSubmissionDTOs;

namespace SolveIt.Application.Submissions.Queries.GetMySubmissions;

public sealed record GetMySubmissionsQuery(Guid ParticipantId)
    : IRequest<List<SubmissionStatusDto>>;
