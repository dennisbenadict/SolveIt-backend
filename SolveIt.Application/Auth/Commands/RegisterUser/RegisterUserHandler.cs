using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Participants;

namespace SolveIt.Application.Auth.Commands.RegisterUser;

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

        if (await _participantRepository
                .GetByPhoneAsync(normalizedPhone, cancellationToken) is not null)
            throw new PhoneAlreadyExistsException();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

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

