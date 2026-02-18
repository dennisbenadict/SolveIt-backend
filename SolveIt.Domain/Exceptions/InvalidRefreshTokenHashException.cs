namespace Solvelt.Domain.Exceptions;

public sealed class InvalidRefreshTokenHashException : DomainException
{
    public InvalidRefreshTokenHashException()
        : base("Token hash is required.")
    {
    }
}

