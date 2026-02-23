using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Commands.RequestPasswordReset;
using SolveIt.Domain.Organizers;

public sealed class RequestPasswordResetHandler
    : IRequestHandler<RequestPasswordResetCommand, Unit>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RequestPasswordResetHandler(
        IOrganizerRepository organizerRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IJwtTokenService jwtTokenService)
    {
        _organizerRepository = organizerRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Unit> Handle(
        RequestPasswordResetCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var organizer =
            await _organizerRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

        // No user enumeration
        if (organizer is null)
            return Unit.Value;

        var rawToken = _jwtTokenService.GenerateRefreshToken();
        var hashedToken = _jwtTokenService.HashRefreshToken(rawToken);

        var resetToken = PasswordResetToken.Create(
            organizer.Id,
            hashedToken,
            DateTime.UtcNow.AddMinutes(15));

        await _passwordResetTokenRepository
            .AddAsync(resetToken, cancellationToken);

        await _passwordResetTokenRepository
            .SaveChangesAsync(cancellationToken);

        // In production: send email
        // For now: log it or temporarily return it if needed

        return Unit.Value;
    }
}

