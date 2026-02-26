using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Common;
using SolveIt.Domain.Organizers;
using SolveIt.Domain.Participants;

namespace SolveIt.Application.Auth.Commands;

public sealed class AuthenticateUserHandler
    : IRequestHandler<AuthenticateUserCommand, AuthResponseDto>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IJwtTokenService _jwtService;
    private readonly IRefreshTokenRepository _refreshRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthenticateUserHandler> _logger;

    public AuthenticateUserHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IJwtTokenService jwtService,
        IRefreshTokenRepository refreshRepository,
        IUnitOfWork unitOfWork,
        ILogger<AuthenticateUserHandler> logger)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _jwtService = jwtService;
        _refreshRepository = refreshRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        var identifier = request.Identifier.Trim();

        Guid userId;
        string email;
        string name;
        string passwordHash;
        UserRole role;

        Organizer? organizer = null;
        Participant? participant = null;

        if (identifier.Contains("@"))
        {
            organizer = await _organizerRepository
                .GetByEmailAsync(identifier.ToLowerInvariant(), cancellationToken);

            if (organizer is null)
            {
                participant = await _participantRepository
                    .GetByEmailAsync(identifier.ToLowerInvariant(), cancellationToken);
            }
        }
        else
        {
            var normalizedPhone = NormalizePhone(identifier);

            organizer = await _organizerRepository
                .GetByPhoneAsync(normalizedPhone, cancellationToken);

            if (organizer is null)
            {
                participant = await _participantRepository
                    .GetByPhoneAsync(normalizedPhone, cancellationToken);
            }
        }

        if (organizer is null && participant is null)
            throw new InvalidCredentialsException();

        // Account lockout check
        if (organizer is not null)
        {
            if (organizer.IsLockedOut())
                throw new AccountLockedException();

            userId = organizer.Id;
            email = organizer.Email;
            name = organizer.Name;
            passwordHash = organizer.PasswordHash;
            role = organizer.Role;
        }
        else
        {
            if (participant!.IsLockedOut())
                throw new AccountLockedException();

            userId = participant.Id;
            email = participant.Email;
            name = participant.Name;
            passwordHash = participant.PasswordHash;
            role = participant.Role;
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            passwordHash);

        if (!passwordValid)
        {
            if (organizer is not null)
                organizer.RegisterFailedLogin();
            else
                participant!.RegisterFailedLogin();

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict during login for user {UserId}",
                    userId);

                throw new InvalidCredentialsException();
            }

            throw new InvalidCredentialsException();
        }

        if (organizer is not null)
            organizer.RegisterSuccessfulLogin();
        else
            participant!.RegisterSuccessfulLogin();

        var accessToken = _jwtService.GenerateAccessToken(
            userId,
            email,
            name,
            role);

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _jwtService.HashRefreshToken(refreshToken);

        var refreshEntity = RefreshToken.Create(
            userId,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(7));

        await _refreshRepository.AddAsync(refreshEntity, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Concurrency conflict during refresh token creation for user {UserId}",
                userId);

            throw new InvalidCredentialsException();
        }

        return new AuthResponseDto(
            accessToken,
            refreshToken);
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

