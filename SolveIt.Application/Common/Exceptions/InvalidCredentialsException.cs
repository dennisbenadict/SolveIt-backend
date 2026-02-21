using SolveIt.Application.Common.Exceptions;

namespace SolveIt.Application.Organizers.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Invalid credentials.")
    {
    }
}

