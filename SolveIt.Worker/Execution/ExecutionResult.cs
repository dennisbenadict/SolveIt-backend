namespace SolveIt.Worker.Execution;

public sealed class ExecutionResult
{
    public bool IsSuccess { get; }
    public bool IsTimeout { get; }
    public string Output { get; }
    public string Error { get; }
    public long TimeMs { get; }

    private ExecutionResult(
        bool success,
        bool timeout,
        string output,
        string error,
        long timeMs)
    {
        IsSuccess = success;
        IsTimeout = timeout;
        Output = output;
        Error = error;
        TimeMs = timeMs;
    }

    public static ExecutionResult CreateSuccess(string output, long timeMs)
        => new(true, false, output, "", timeMs);

    public static ExecutionResult CreateRuntimeError(string error)
        => new(false, false, "", error, 0);

    public static ExecutionResult CreateTimeout()
        => new(false, true, "", "Time limit exceeded", 0);
}
