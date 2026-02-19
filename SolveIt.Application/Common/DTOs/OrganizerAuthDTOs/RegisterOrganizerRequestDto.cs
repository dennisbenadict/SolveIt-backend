using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Common.DTOs.OrganizerAuthDTOs
{
    public sealed record RegisterOrganizerRequestDto(
        string Email,
        string Name,
        string PhoneNumber,
        string Password,
        string ConfirmPassword);
}
