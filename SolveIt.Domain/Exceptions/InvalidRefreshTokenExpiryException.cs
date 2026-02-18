namespace Solvelt.Domain.Exceptions;

public sealed class InvalidRefreshTokenExpiryException : DomainException
{
    public InvalidRefreshTokenExpiryException()
        : base("Refresh token expiry must be in the future.")
    {
    }
}
