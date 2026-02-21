using BCrypt.Net;
using MediatR;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Organizers;
using System.Security.Authentication;

namespace SolveIt.Application.Organizers.Commands.LoginOrganizer;

public sealed class LoginOrganizerHandler
    : IRequestHandler<LoginOrganizerCommand, AuthResponseDto>
{
    private readonly IOrganizerRepository _repository;
    private readonly IJwtTokenService _jwtService;
    private readonly IRefreshTokenRepository _refreshRepository;

    public LoginOrganizerHandler(
        IOrganizerRepository repository,
        IJwtTokenService jwtService,
        IRefreshTokenRepository refreshRepository)
    {
        _repository = repository;
        _jwtService = jwtService;
        _refreshRepository = refreshRepository;
    }

    public async Task<AuthResponseDto> Handle(
        LoginOrganizerCommand request,
        CancellationToken cancellationToken)
    {
        var identifier = request.Identifier.Trim();

        Organizer? organizer;

        if (identifier.Contains("@"))
        {
            organizer = await _repository
                .GetByEmailAsync(identifier.ToLowerInvariant(), cancellationToken);
        }
        else
        {
            var normalizedPhone = NormalizePhone(identifier);

            organizer = await _repository
                .GetByPhoneAsync(normalizedPhone, cancellationToken);
        }

        if (organizer is null)
            throw new InvalidCredentialsException();

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            organizer.PasswordHash);

        if (!passwordValid)
            throw new InvalidCredentialsException();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(organizer);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = _jwtService.HashRefreshToken(refreshToken);

        // CREATE refresh token entity
        var refreshEntity = RefreshToken.Create(
            organizer.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(7) // production expiry
        );

        // Persist refresh token
        await _refreshRepository.AddAsync(refreshEntity, cancellationToken);
        await _refreshRepository.SaveChangesAsync(cancellationToken);

        // TODO: Save refreshTokenHash in DB (next step)

        // Return RAW tokens (only once)
        return new AuthResponseDto(
            accessToken,
            refreshToken
        );
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
