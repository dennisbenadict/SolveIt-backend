using FluentValidation;
using SolveIt.Domain.Common;
using System.Text.RegularExpressions;

namespace SolveIt.Application.Auth.Commands.RegisterUser;

public sealed class RegisterUserValidator
    : AbstractValidator<RegisterUserCommand>
{
    private static readonly Regex IndianPhoneRegex =
        new(@"^(?:\+91|91)?[6-9]\d{9}$", RegexOptions.Compiled);

    private static readonly Regex NameRegex =
        new(@"^[a-zA-Z0-9\s\.\-']+$", RegexOptions.Compiled);

    private static readonly Regex EmailRegex =
        new(@"^[A-Za-z0-9](?!.*[._-]{2})[A-Za-z0-9._-]*[A-Za-z0-9]@[A-Za-z0-9-]+\.[A-Za-z]{2,}$",
            RegexOptions.Compiled);

    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150)
            .Matches(NameRegex)
            .WithMessage("Name contains invalid characters.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(254)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .Matches(EmailRegex)
            .WithMessage("Email cannot start or end with special characters.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(p => IndianPhoneRegex.IsMatch(p))
            .WithMessage("Phone number must be a valid Indian mobile number.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage("Must contain uppercase.")
            .Matches("[a-z]").WithMessage("Must contain lowercase.")
            .Matches("[0-9]").WithMessage("Must contain digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Must contain special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}
