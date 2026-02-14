using FluentValidation;
using System.Text.RegularExpressions;

public sealed class RegisterOrganizerValidator
	: AbstractValidator<RegisterOrganizerCommand>
{
	private static readonly Regex PhoneRegex =
		new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);

	public RegisterOrganizerValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.MaximumLength(150);

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.Must(Email => !string.IsNullOrWhiteSpace(Email?.Trim()))
			.WithMessage("Email cannot be empty or spaces.")
			.Must(email => email!.Trim().Length > 0)
			.WithMessage("Email cannot be blank.")
			.Must(email => RegistrationValidationHelper.IsValidEmail(email!.Trim()))
			.WithMessage("Invalid email format.");
			.EmailAddress()
			.MaximumLength(254);

		RuleFor(x => x.PhoneNumber)
			.NotEmpty()
			.Must(p => PhoneRegex.IsMatch(p))
			.WithMessage("Phone number must be in E.164 format.");
	}
}

using FluentValidation;
using System.Text.RegularExpressions;

namespace SolveIt.Application.Organizers.Commands.RegisterOrganizer;

public sealed class RegisterOrganizerValidator
	: AbstractValidator<RegisterOrganizerCommand>
{
	private static readonly Regex IndianPhoneRegex =
	new(@"^(?:\+91)?[6-9]\d{9}$", RegexOptions.Compiled);


	private static readonly Regex NameRegex =
		new(@"^[a-zA-Z0-9\s\.\-']+$", RegexOptions.Compiled);

	public RegisterOrganizerValidator()
	{
		// NAME
		RuleFor(x => x.Name)
			.Cascade(CascadeMode.Stop)
			.NotEmpty().WithMessage("Name is required.")
			.MaximumLength(150)
			.Matches(NameRegex)
			.WithMessage("Name contains invalid characters.");

		// EMAIL
		RuleFor(x => x.Email)
			.Cascade(CascadeMode.Stop)
			.NotEmpty().WithMessage("Email is required.")
			.MaximumLength(254)
			.EmailAddress()
			.WithMessage("Invalid email format.");

		// PHONE (E.164)
		RuleFor(x => x.PhoneNumber)
			.Cascade(CascadeMode.Stop)
			.NotEmpty().WithMessage("Phone number is required.")
			.MaximumLength(13)
			.Must(p => PhoneRegex.IsMatch(p))
			.WithMessage("Phone number must be in valid E.164 format.");
	}
}



