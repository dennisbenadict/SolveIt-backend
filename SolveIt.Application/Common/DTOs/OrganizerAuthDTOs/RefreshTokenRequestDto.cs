using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.OrganizerAuthDTOs
{
    public sealed record RefreshTokenRequestDto(
        string RefreshToken);
    //This Dto will also be used for Logout Request
}
