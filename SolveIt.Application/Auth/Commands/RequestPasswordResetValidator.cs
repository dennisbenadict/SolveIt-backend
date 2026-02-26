using FluentValidation;
using SolveIt.Application.Auth.Commands;

namespace SolveIt.Application.Auth.Validators;

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
