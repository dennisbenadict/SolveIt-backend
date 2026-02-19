using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.FinalSubmissionDTOs
{
    public sealed record SubmitSolutionRequest(
        Guid TournamentId,
        Guid ProblemId,
        string Code);
}
