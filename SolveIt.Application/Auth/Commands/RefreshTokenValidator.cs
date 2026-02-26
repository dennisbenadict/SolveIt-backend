using FluentValidation;
using SolveIt.Application.Auth.Commands;

namespace SolveIt.Application.Auth.Validators;

public sealed class RefreshTokenValidator
    : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}