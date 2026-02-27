using FluentValidation;

namespace SolveIt.Application.Auth.Commands.RequestPasswordReset;

public sealed class RequestPasswordResetValidator
    : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }
}
