using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Solvelt.Application.Common.Interfaces;
using Solvelt.Application.Organizers.Commands.LoginOrganizer;
using Solvelt.Domain.Organizers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;


namespace Solvelt.Infrastructure.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponse GenerateToken(Organizer organizer)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var secretKey = jwtSection["SecretKey"];
        var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"]!);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtClaimNames.Sub, organizer.Id.ToString()),
            new Claim(JwtClaimNames.Email, organizer.Email),
            new Claim("name", organizer.Name)
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(tokenString, expiresAt);
    }
}

