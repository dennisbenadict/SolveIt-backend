using SolveIt.Application.Common.Exceptions;

namespace Solvelt.Application.Organizers.Exceptions;

public sealed class PhoneAlreadyExistsException : DomainException
{
    public PhoneAlreadyExistsException()
        : base("Phone number is already registered.")
    {
    }
}

