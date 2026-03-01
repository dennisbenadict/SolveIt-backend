using MediatR;
using Microsoft.EntityFrameworkCore;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Common;
using SolveIt.Domain.Organizers;
using DomainRefreshToken = SolveIt.Domain.Organizers.RefreshToken;

namespace SolveIt.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var hashedToken =
            _jwtTokenService.HashRefreshToken(request.RefreshToken);

        var storedToken =
            await _refreshTokenRepository
                .GetByHashAsync(hashedToken, cancellationToken);

        if (storedToken is null)
            throw new InvalidCredentialsException();

        if (storedToken.IsRevoked)
        {
            if (storedToken.ReplacedByTokenId is not null)
            {
                await _refreshTokenRepository
                    .RevokeAllByUserIdAsync(
                        storedToken.UserId,
                        cancellationToken);

                await _refreshTokenRepository
                    .SaveChangesAsync(cancellationToken);

                throw new RefreshTokenReuseDetectedException();
            }

            throw new InvalidCredentialsException();
        }

        if (storedToken.ExpiresAtUtc <= now)
            throw new InvalidCredentialsException();

        Guid userId = storedToken.UserId;
        string email;
        string name;
        UserRole role;

        var organizer =
            await _organizerRepository
                .GetByIdAsync(userId, cancellationToken);

        if (organizer is not null)
        {
            if (organizer.IsLockedOut())
                throw new AccountLockedException();

            if (organizer.IsBlocked)
            {
                storedToken.Revoke(null);
                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
                throw new AccountBlockedException();
            }

            email = organizer.Email;
            name = organizer.Name;
            role = organizer.Role;
        }
        else
        {
            var participant =
                await _participantRepository
                    .GetByIdAsync(userId, cancellationToken);

            if (participant is null)
                throw new InvalidCredentialsException();

            if (participant.IsLockedOut())
                throw new AccountLockedException();

            if (participant.IsBlocked)
            {
                storedToken.Revoke(null);
                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
                throw new AccountBlockedException();
            }

            email = participant.Email;
            name = participant.Name;
            role = participant.Role;
        }

        var newAccessToken =
            _jwtTokenService.GenerateAccessToken(
                userId,
                email,
                name,
                role);

        var rawRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var newHashedToken =
            _jwtTokenService.HashRefreshToken(rawRefreshToken);

        var newRefreshToken =
            DomainRefreshToken.Create(
                userId,
                newHashedToken,
                now.AddDays(7),
                storedToken.DeviceFingerprint,
                storedToken.IpAddress,
                storedToken.UserAgent);

        storedToken.Revoke(newRefreshToken.Id);

        await _refreshTokenRepository
            .AddAsync(newRefreshToken, cancellationToken);

        try
        {
            await _refreshTokenRepository
                .SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidCredentialsException();
        }

        return new AuthResponseDto(
            newAccessToken,
            rawRefreshToken);
    }
}

