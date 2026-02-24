using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;

public sealed class ResetPasswordHandler
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IOrganizerRepository organizerRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _organizerRepository = organizerRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
            throw new PasswordsDoNotMatchException();

        var hashedToken =
            _jwtTokenService.HashRefreshToken(request.Token);

        var storedToken =
            await _passwordResetTokenRepository
                .GetByHashAsync(hashedToken, cancellationToken);

        if (storedToken is null || storedToken.IsUsed ||
            storedToken.ExpiresAtUtc < DateTime.UtcNow)
            throw new InvalidPasswordResetTokenException();
        storedToken.Use(); // Domain enforces expiry + reuse

        var organizer =
            await _organizerRepository
                .GetByIdAsync(storedToken.OrganizerId, cancellationToken);

        if (organizer is null)
            throw new OrganizerNotFoundException();

        var newPasswordHash =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        organizer.UpdatePassword(newPasswordHash);

        // Kill all sessions after password change
        await _refreshTokenRepository
            .RevokeAllByOrganizerIdAsync(
                organizer.Id,
                cancellationToken);

        // Save everything once
        try
        {
            await _passwordResetTokenRepository
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidPasswordResetTokenException();
        }

        return Unit.Value;
    }
}

