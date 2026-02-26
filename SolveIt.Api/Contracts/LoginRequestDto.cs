namespace SolveIt.Api.Contracts;

public sealed class LoginRequestDto
{
    public string Identifier { get; init; } = null!;
    public string Password { get; init; } = null!;
}

