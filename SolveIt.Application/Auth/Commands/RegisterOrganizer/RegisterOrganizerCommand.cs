using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Auth.Commands.RegisterOrganizer
{
    public sealed record RegisterOrganizerCommand(
        string Name,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfirmPassword
    ) : IRequest<Guid>;
}
