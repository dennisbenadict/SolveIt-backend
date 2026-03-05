namespace SolveIt.Domain.Submissions;

public enum SubmissionStatus
{
    Pending = 0,            // Created but not queued
    Queued = 1,             // Sent to worker queue
    Running = 2,            // Executing

    Accepted = 3,           // Passed all tests
    WrongAnswer = 4,        // Output mismatch
    RuntimeError = 5,       // Program crashed
    CompilationError = 6,   // Build failed
    TimeLimitExceeded = 7,  // Execution timeout
    MemoryLimitExceeded = 8 // Memory violation
}
