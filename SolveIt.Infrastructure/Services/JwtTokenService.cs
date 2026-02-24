using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Domain.Organizers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SolveIt.Infrastructure.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(Organizer organizer)
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

        //var claims = new[]
        //{
        //    new Claim(JwtClaimNames.Sub, organizer.Id.ToString()),
        //    new Claim(JwtClaimNames.Email, organizer.Email),
        //    new Claim("name", organizer.Name)
        //};

        var claims = new[]
          {
              new Claim(ClaimTypes.NameIdentifier, organizer.Id.ToString()),
              new Claim(ClaimTypes.Email, organizer.Email),
              new Claim(ClaimTypes.Name, organizer.Name),
              new Claim(ClaimTypes.Role, organizer.Role.ToString())
          };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        // 256-bit secure random token (32 bytes)
        var randomBytes = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public string HashRefreshToken(string refreshToken)
    {
        using var sha = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToHexString(hash); // 64 char hex
    }
}
