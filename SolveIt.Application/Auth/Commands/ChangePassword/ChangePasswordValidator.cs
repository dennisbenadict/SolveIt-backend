using FluentValidation;

namespace SolveIt.Application.Auth.Commands.ChangePassword;

public sealed class ChangePasswordValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage("Must contain uppercase.")
            .Matches("[a-z]").WithMessage("Must contain lowercase.")
            .Matches("[0-9]").WithMessage("Must contain digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character.");
    }
}
