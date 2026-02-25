using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Participants;

namespace SolveIt.Application.Participants.Commands.RegisterParticipant;

public sealed class RegisterParticipantHandler
    : IRequestHandler<RegisterParticipantCommand, Guid>
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IOrganizerRepository _organizerRepository;

    public RegisterParticipantHandler(
        IParticipantRepository participantRepository,
        IOrganizerRepository organizerRepository)
    {
        _participantRepository = participantRepository;
        _organizerRepository = organizerRepository;
    }

    public async Task<Guid> Handle(
        RegisterParticipantCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
            throw new PasswordsDoNotMatchException();

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedPhone = request.PhoneNumber.Trim();

        // Global email uniqueness check
        var organizerWithEmail =
            await _organizerRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

        if (organizerWithEmail is not null)
            throw new EmailAlreadyExistsException();

        var participantWithEmail =
            await _participantRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

        if (participantWithEmail is not null)
            throw new EmailAlreadyExistsException();

        // Phone uniqueness (within participants)
        var participantWithPhone =
            await _participantRepository
                .GetByPhoneAsync(normalizedPhone, cancellationToken);

        if (participantWithPhone is not null)
            throw new PhoneAlreadyExistsException();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var participant = Participant.Create(
            request.Name,
            normalizedEmail,
            normalizedPhone,
            passwordHash);

        await _participantRepository
            .AddAsync(participant, cancellationToken);

        await _participantRepository
            .SaveChangesAsync(cancellationToken);

        return participant.Id;
    }
}
