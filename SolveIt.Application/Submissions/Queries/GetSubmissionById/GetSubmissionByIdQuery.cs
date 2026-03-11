using MediatR;
using SolveIt.Application.Common.DTOs.FinalSubmissionDTOs;

namespace SolveIt.Application.Submissions.Queries.GetSubmissionById;

public sealed record GetSubmissionByIdQuery(Guid SubmissionId)
    : IRequest<SubmissionStatusDto>;
