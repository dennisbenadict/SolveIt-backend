using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.OrganizerAuthDTOs
{
    public sealed record OrganizerProfileDto(
        string Email,
        string Name);
}
