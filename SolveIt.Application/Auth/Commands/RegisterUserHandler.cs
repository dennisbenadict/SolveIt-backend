using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Common;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Participants;

namespace SolveIt.Application.Auth.Commands;

public sealed class RegisterUserHandler
    : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
            throw new PasswordsDoNotMatchException();

        if (request.Role == UserRole.SuperAdmin)
            throw new InvalidOperationException("SuperAdmin registration is not allowed via API.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedName = request.Name.Trim();
        var rawPhone = request.PhoneNumber.Trim();

        // Global email uniqueness across organizers and participants
        if (await _organizerRepository
                .ExistsByEmailAsync(normalizedEmail, cancellationToken))
            throw new EmailAlreadyExistsException();

        var participantWithEmail =
            await _participantRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

        if (participantWithEmail is not null)
            throw new EmailAlreadyExistsException();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var normalizedPhone = NormalizePhone(rawPhone);

        if (request.Role == UserRole.Organizer)
        {
            if (await _organizerRepository
                    .ExistsByPhoneAsync(normalizedPhone, cancellationToken))
            {
                throw new PhoneAlreadyExistsException();
            }

            var organizer = Organizer.Create(
                normalizedName,
                normalizedEmail,
                normalizedPhone,
                passwordHash);

            await _organizerRepository.AddAsync(organizer, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return organizer.Id;
        }

        // Participant registration
        var participantWithPhone =
            await _participantRepository
                .GetByPhoneAsync(normalizedPhone, cancellationToken);

        if (participantWithPhone is not null)
            throw new PhoneAlreadyExistsException();

        var participant = Participant.Create(
            normalizedName,
            normalizedEmail,
            normalizedPhone,
            passwordHash);

        await _participantRepository
            .AddAsync(participant, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return participant.Id;
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

