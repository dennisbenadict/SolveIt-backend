using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.ParticipantDTOs
{
    public sealed record ParticipantAccessRequest(
        string Email,
        string Name,
        string Password,
        Guid TournamentId);
}
