using SolveIt.Application.Common.Exceptions;

namespace Solvelt.Application.Organizers.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Invalid credentials.")
    {
    }
}

