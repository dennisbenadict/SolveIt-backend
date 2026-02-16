using BCrypt.Net;
using MediatR;
using Solvelt.Application.Common.Interfaces;
using Solvelt.Application.Interfaces;
using Solvelt.Application.Organizers.Exceptions;
using Solvelt.Domain.Organizers;
using System.Security.Authentication;

namespace Solvelt.Application.Organizers.Commands.LoginOrganizer;

public sealed class LoginOrganizerHandler
    : IRequestHandler<LoginOrganizerCommand, LoginResponse>
{
    private readonly IOrganizerRepository _repository;
    private readonly IJwtTokenService _jwtService;

    public LoginOrganizerHandler(
        IOrganizerRepository repository,
        IJwtTokenService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
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

        return _jwtService.GenerateToken(organizer);
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
