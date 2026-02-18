namespace Solvelt.Application.Organizers.Commands.LoginOrganizer;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken
);


