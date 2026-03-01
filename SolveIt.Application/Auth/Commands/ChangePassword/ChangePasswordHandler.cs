using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var organizer = await _organizerRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (organizer is not null)
        {
            if (!BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                organizer.PasswordHash))
            {
                throw new InvalidCredentialsException();
            }

            // Prevent same password reuse
            if (BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                organizer.PasswordHash))
            {
                throw new PasswordReuseNotAllowedException();
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            organizer.UpdatePassword(newHash);

            organizer.RegisterSuccessfulLogin(); // reset lockout state

            await _refreshTokenRepository
                .RevokeAllByUserIdAsync(
                    organizer.Id,
                    cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var participant = await _participantRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (participant is null)
            throw new UserNotFoundException();

        if (!BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword,
            participant.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        if (BCrypt.Net.BCrypt.Verify(
            request.NewPassword,
            participant.PasswordHash))
        {
            throw new PasswordReuseNotAllowedException();
        }

        var newHashParticipant =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        participant.UpdatePassword(newHashParticipant);

        participant.RegisterSuccessfulLogin();

        await _refreshTokenRepository
            .RevokeAllByUserIdAsync(
                participant.Id,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}