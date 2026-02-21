namespace SolveIt.Domain.Exceptions;

public sealed class InvalidRefreshTokenOrganizerException : DomainException
{
    public InvalidRefreshTokenOrganizerException()
        : base("OrganizerId cannot be empty.")
    {
    }
}

