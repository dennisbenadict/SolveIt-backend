using MediatR;
using SolveIt.Application.Common.DTOs;
using SolveIt.Application.Common.DTOs.FinalSubmissionDTOs;
using SolveIt.Application.Interfaces;

namespace SolveIt.Application.Submissions.Queries.GetMySubmissions;

public sealed class GetMySubmissionsHandler
    : IRequestHandler<GetMySubmissionsQuery, List<SubmissionStatusDto>>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GetMySubmissionsHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<List<SubmissionStatusDto>> Handle(
        GetMySubmissionsQuery request,
        CancellationToken cancellationToken)
    {
        var submissions = await _submissionRepository
            .GetByParticipantAsync(request.ParticipantId, cancellationToken);

        return submissions.Select(s => new SubmissionStatusDto
        {
            Id = s.Id,
            Status = s.Status,
            PassedTestCases = s.PassedTestCases,
            TotalTestCases = s.TotalTestCases,
            CreatedAtUtc = s.CreatedAtUtc
        }).ToList();
    }
}
