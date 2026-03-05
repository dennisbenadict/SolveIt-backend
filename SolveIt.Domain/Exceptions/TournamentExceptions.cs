using SolveIt.Domain.Exceptions;

namespace SolveIt.Domain.Exceptions;
public sealed class TournamentTitleRequiredException : DomainException
{
    public TournamentTitleRequiredException()
        : base("Tournament title is required.") { }
}

public sealed class TournamentInvalidTimeRangeException : DomainException
{
    public TournamentInvalidTimeRangeException()
        : base("Start time must be before end time.") { }
}

public sealed class TournamentNotDraftException : DomainException
{
    public TournamentNotDraftException()
        : base("Only draft tournaments can be published.") { }
}

public sealed class TournamentNotPublishedException : DomainException
{
    public TournamentNotPublishedException()
        : base("Tournament must be published first.") { }
}

public sealed class TournamentNotOngoingException : DomainException
{
    public TournamentNotOngoingException()
        : base("Tournament must be ongoing to complete.") { }
}

public sealed class TournamentAlreadyCompletedException : DomainException
{
    public TournamentAlreadyCompletedException()
        : base("Completed tournaments cannot be cancelled.") { }
}

public sealed class TournamentNotFoundException : DomainException
{
    public TournamentNotFoundException()
        : base("Tournament not found.") { }
}

public sealed class TournamentNotOwnedException : DomainException
{
    public TournamentNotOwnedException()
        : base("You do not own this tournament.") { }
}

public sealed class TournamentNotOpenException : DomainException
{
    public TournamentNotOpenException()
        : base("Tournament is not open for participants.") { }
}

public sealed class AlreadyJoinedException : DomainException
{
    public AlreadyJoinedException()
        : base("You have already joined this tournament.") { }
}