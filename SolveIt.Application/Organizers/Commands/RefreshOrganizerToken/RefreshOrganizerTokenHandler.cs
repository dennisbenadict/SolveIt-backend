using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Organizers;

namespace SolveIt.Application.Organizers.Commands.RefreshOrganizerToken;

public sealed class RefreshOrganizerTokenHandler
    : IRequestHandler<RefreshOrganizerTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshOrganizerTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IOrganizerRepository organizerRepository,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _organizerRepository = organizerRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(
        RefreshOrganizerTokenCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Hash incoming token
        var hashedToken =
            _jwtTokenService.HashRefreshToken(request.RefreshToken);

        // Find stored token
        var storedToken =
            await _refreshTokenRepository
                .GetByHashAsync(hashedToken, cancellationToken);

        if (storedToken is null)
            throw new InvalidCredentialsException();

        // Reuse detection
        if (storedToken.IsRevoked)
        {
            if (storedToken.ReplacedByTokenId is not null)
            {
                await _refreshTokenRepository
                    .RevokeAllByOrganizerIdAsync(
                        storedToken.OrganizerId,
                        cancellationToken);

                await _refreshTokenRepository
                    .SaveChangesAsync(cancellationToken);

                throw new RefreshTokenReuseDetectedException();
            }

            throw new InvalidCredentialsException();
        }

        // Expiry check
        if (storedToken.ExpiresAtUtc <= now)
            throw new InvalidCredentialsException();

        // Load organizer
        var organizer =
            await _organizerRepository
                .GetByIdAsync(storedToken.OrganizerId, cancellationToken);

        if (organizer is null)
            throw new InvalidCredentialsException();

        // Enforce lockout on refresh
        if (organizer.IsLockedOut())
            throw new AccountLockedException();

        // Generate new tokens
        var newAccessToken =
            _jwtTokenService.GenerateAccessToken(organizer);

        var rawRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var newHashedToken =
            _jwtTokenService.HashRefreshToken(rawRefreshToken);

        var newRefreshToken =
            RefreshToken.Create(
                organizer.Id,
                newHashedToken,
                now.AddDays(7));

        // Rotate properly
        storedToken.Revoke(newRefreshToken.Id);

        await _refreshTokenRepository
            .AddAsync(newRefreshToken, cancellationToken);

        // Save once
        await _refreshTokenRepository
            .SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            newAccessToken,
            rawRefreshToken);
    }
}
