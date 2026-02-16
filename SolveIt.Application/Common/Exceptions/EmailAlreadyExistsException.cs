using SolveIt.Application.Common.Exceptions;

namespace Solvelt.Application.Organizers.Exceptions;

public sealed class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException()
        : base("Email is already registered.")
    {
    }
}
