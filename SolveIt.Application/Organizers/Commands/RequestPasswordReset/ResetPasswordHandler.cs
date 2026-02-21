using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Interfaces;
using Solvelt.Application.Common.Interfaces;
using Solvelt.Application.Interfaces;

public sealed class ResetPasswordHandler
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IOrganizerRepository organizerRepository,
        IJwtTokenService jwtTokenService)
    {
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _organizerRepository = organizerRepository;
        _jwtTokenService = jwtTokenService;
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

        if (storedToken is null)
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

        await _passwordResetTokenRepository
            .SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

