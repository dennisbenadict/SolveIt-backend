using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.AutoSaveDTOs
{
    public sealed record AutosaveSubmissionRequest(
        Guid TournamentId,
        Guid ProblemId,
        string Language,
        string Code);

}
