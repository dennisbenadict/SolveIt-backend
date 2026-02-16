using FluentValidation;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Solvelt.Application.Organizers.Commands.LoginOrganizer;

public sealed class LoginOrganizerValidator
    : AbstractValidator<LoginOrganizerCommand>
{
    private static readonly Regex IndianPhoneRegex =
        new(@"^(?:\+91|91)?[6-9]\d{9}$", RegexOptions.Compiled);

    public LoginOrganizerValidator()
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

        // If contains '@' treat as email
        if (identifier.Contains("@"))
        {
            try
            {
                var mail = new MailAddress(identifier);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Otherwise treat as Indian phone
        return IndianPhoneRegex.IsMatch(identifier);
    }
}

