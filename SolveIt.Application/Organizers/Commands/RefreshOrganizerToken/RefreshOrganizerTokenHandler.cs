using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.Exceptions;
using Solvelt.Application.Common.Interfaces;
using Solvelt.Application.Interfaces;
using Solvelt.Application.Organizers.Exceptions;
using Solvelt.Domain.Organizers;

namespace Solvelt.Application.Organizers.Commands.RefreshOrganizerToken;

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
            await _refreshTokenRepository
                .RevokeAllByOrganizerIdAsync(
                    storedToken.OrganizerId,
                    cancellationToken);

            await _refreshTokenRepository
                .SaveChangesAsync(cancellationToken);

            throw new RefreshTokenReuseDetectedException();
        }

        // Expiry check
        if (storedToken.ExpiresAtUtc <= DateTime.UtcNow)
            throw new InvalidCredentialsException();

        // Load organizer
        var organizer =
            await _organizerRepository
                .GetByIdAsync(storedToken.OrganizerId, cancellationToken);

        if (organizer is null)
            throw new InvalidCredentialsException();

        // Rotate (revoke old)
        storedToken.Revoke();

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
                DateTime.UtcNow.AddDays(7));

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
