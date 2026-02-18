using FluentValidation;

namespace Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;

public sealed class RefreshOrganizerTokenValidator
    : AbstractValidator<RefreshOrganizerTokenCommand>
{
    public RefreshOrganizerTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}

