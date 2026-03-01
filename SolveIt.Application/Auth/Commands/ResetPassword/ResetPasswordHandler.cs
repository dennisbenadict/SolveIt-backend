using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Auth.Commands.ResetPassword;

public sealed class ResetPasswordHandler
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordsDoNotMatchException();

        var hashedToken = _jwtTokenService.HashRefreshToken(request.Token);

        var storedToken = await _passwordResetTokenRepository
            .GetByHashAsync(hashedToken, cancellationToken);

        if (storedToken is null)
            throw new PasswordResetTokenExpiredException();

        storedToken.Use();

        var organizer = await _organizerRepository
            .GetByIdAsync(storedToken.OrganizerId, cancellationToken);

        Guid userId;

        if (organizer is not null)
        {
            organizer.UpdatePassword(
                BCrypt.Net.BCrypt.HashPassword(request.NewPassword));

            userId = organizer.Id;
        }
        else
        {
            var participant = await _participantRepository
                .GetByIdAsync(storedToken.OrganizerId, cancellationToken);

            if (participant is null)
                throw new PasswordResetTokenExpiredException();

            participant.UpdatePassword(
                BCrypt.Net.BCrypt.HashPassword(request.NewPassword));

            userId = participant.Id;
        }

        await _refreshTokenRepository
            .RevokeAllByUserIdAsync(userId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
