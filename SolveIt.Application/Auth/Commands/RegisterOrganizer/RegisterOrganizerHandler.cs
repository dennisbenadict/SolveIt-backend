using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Organizers;

namespace SolveIt.Application.Auth.Commands.RegisterOrganizer;

public sealed class RegisterOrganizerHandler
    : IRequestHandler<RegisterOrganizerCommand, Guid>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterOrganizerHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        RegisterOrganizerCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
            throw new PasswordsDoNotMatchException();

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedName = request.Name.Trim();
        var normalizedPhone = NormalizePhone(request.PhoneNumber);

        // Global email uniqueness
        if (await _organizerRepository
                .ExistsByEmailAsync(normalizedEmail, cancellationToken))
            throw new EmailAlreadyExistsException();

        if (await _participantRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken) is not null)
            throw new EmailAlreadyExistsException();

        // Organizer phone uniqueness
        if (await _organizerRepository
                .ExistsByPhoneAsync(normalizedPhone, cancellationToken))
            throw new PhoneAlreadyExistsException();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var organizer = Organizer.Create(
            normalizedName,
            normalizedEmail,
            normalizedPhone,
            passwordHash);

        await _organizerRepository.AddAsync(organizer, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizer.Id;
    }

    private static string NormalizePhone(string input)
    {
        input = input.Trim();

        if (input.StartsWith("+91"))
            return input;

        if (input.StartsWith("91") && input.Length == 12)
            return "+" + input;

        return "+91" + input;
    }
}
