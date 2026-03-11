using MediatR;
using SolveIt.Application.Common.DTOs;
using SolveIt.Application.Common.DTOs.FinalSubmissionDTOs;
using SolveIt.Application.Interfaces;

namespace SolveIt.Application.Submissions.Queries.GetSubmissionById;

public sealed class GetSubmissionByIdHandler
    : IRequestHandler<GetSubmissionByIdQuery, SubmissionStatusDto>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GetSubmissionByIdHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<SubmissionStatusDto> Handle(
        GetSubmissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository
            .GetByIdAsync(request.SubmissionId, cancellationToken);

        if (submission == null)
            throw new Exception("Submission not found");

        return new SubmissionStatusDto
        {
            Id = submission.Id,
            Status = submission.Status,
            PassedTestCases = submission.PassedTestCases,
            TotalTestCases = submission.TotalTestCases,
            CreatedAtUtc = submission.CreatedAtUtc
        };
    }
}
