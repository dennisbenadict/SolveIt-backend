using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.TournamentHostingDTOs
{
    public sealed record CreateTournamentRequest(
        string Title,
        DateTime StartTimeUtc,
        DateTime EndTimeUtc);

}
 