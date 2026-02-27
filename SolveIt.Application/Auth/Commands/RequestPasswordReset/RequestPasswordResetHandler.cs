using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Participants;

namespace SolveIt.Application.Auth.Commands.RequestPasswordReset;

public sealed class RequestPasswordResetHandler
    : IRequestHandler<RequestPasswordResetCommand, Unit>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RequestPasswordResetHandler> _logger;

    public RequestPasswordResetHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        ILogger<RequestPasswordResetHandler> logger)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        RequestPasswordResetCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var organizer = await _organizerRepository
            .GetByEmailAsync(normalizedEmail, cancellationToken);

        Guid userId;

        if (organizer is not null)
        {
            userId = organizer.Id;
        }
        else
        {
            var participant = await _participantRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

            if (participant is null)
                return Unit.Value;

            userId = participant.Id;
        }

        var recentCount = await _passwordResetTokenRepository
            .CountRecentRequestsAsync(
                userId,
                DateTime.UtcNow.AddMinutes(-10),
                cancellationToken);

        if (recentCount >= 3)
        {
            _logger.LogWarning(
                "Password reset throttled for user {UserId}",
                userId);

            return Unit.Value;
        }

        await _passwordResetTokenRepository
            .RevokeActiveTokensAsync(userId, cancellationToken);

        var rawToken = _jwtTokenService.GenerateRefreshToken();
        var hashedToken = _jwtTokenService.HashRefreshToken(rawToken);

        var resetToken = PasswordResetToken.Create(
            userId,
            hashedToken,
            DateTime.UtcNow.AddMinutes(15));

        await _passwordResetTokenRepository
            .AddAsync(resetToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
