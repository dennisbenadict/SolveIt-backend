using FluentValidation;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;

namespace Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;

public sealed class RefreshOrganizerTokenValidator
    : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshOrganizerTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}

