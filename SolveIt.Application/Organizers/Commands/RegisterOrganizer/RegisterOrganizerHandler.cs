using MediatR;
using Solvelt.Application.Interfaces;
using Solvelt.Domain.Organizers;

namespace Solvelt.Application.Organizers.Commands.RegisterOrganizer;

public sealed class RegisterOrganizerHandler
    : IRequestHandler<RegisterOrganizerCommand, Guid>
{
    private readonly IOrganizerRepository _organizerRepository;

    public RegisterOrganizerHandler(IOrganizerRepository organizerRepository)
    {
        _organizerRepository = organizerRepository;
    }

    public async Task<Guid> Handle(
        RegisterOrganizerCommand request,
        CancellationToken cancellationToken)
    {
        // Normalize email
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Normalize phone safely
        var rawPhone = request.PhoneNumber.Trim();

        string normalizedPhone;

        if (rawPhone.StartsWith("+91"))
        {
            normalizedPhone = rawPhone;
        }
        else if (rawPhone.StartsWith("91") && rawPhone.Length == 12)
        {
            normalizedPhone = "+" + rawPhone;
        }
        else
        {
            normalizedPhone = "+91" + rawPhone;
        }

        // Email uniqueness
        if (await _organizerRepository
            .ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        // Phone uniqueness
        if (await _organizerRepository
            .ExistsByPhoneAsync(normalizedPhone, cancellationToken))
        {
            throw new InvalidOperationException("Phone number is already registered.");
        }

        var organizer = Organizer.Create(
            request.Name.Trim(),
            normalizedEmail,
            normalizedPhone,
            "local"
        );

        await _organizerRepository.AddAsync(organizer, cancellationToken);

        return organizer.Id;
    }
}

