using FluentValidation;
using SolveIt.Application.Auth.Commands;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SolveIt.Application.Auth.Validators;

public sealed class AuthenticateUserValidator
    : AbstractValidator<AuthenticateUserCommand>
{
    private static readonly Regex IndianPhoneRegex =
        new(@"^(?:\+91|91)?[6-9]\d{9}$", RegexOptions.Compiled);

    public AuthenticateUserValidator()
    {
        RuleFor(x => x.Identifier)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email or phone number is required.")
            .MaximumLength(254)
            .Must(BeValidIdentifier)
            .WithMessage("Invalid email or phone number format.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8)
            .MaximumLength(100);
    }

    private static bool BeValidIdentifier(string identifier)
    {
        identifier = identifier.Trim();

        if (identifier.Contains("@"))
        {
            try
            {
                _ = new MailAddress(identifier);
                return true;
            }
            catch
            {
                return false;
            }
        }

        return IndianPhoneRegex.IsMatch(identifier);
    }
}
