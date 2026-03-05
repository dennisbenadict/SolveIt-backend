namespace SolveIt.Domain.TestCases;

public sealed class TestCase
{
    public Guid Id { get; private set; }

    public Guid TournamentProblemId { get; private set; }

    public string Input { get; private set; } = null!;

    public string ExpectedOutput { get; private set; } = null!;

    public bool IsHidden { get; private set; }

    public int OrderIndex { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public byte[] RowVersion { get; private set; } = null!;

    private TestCase() { }

    private TestCase(
        Guid id,
        Guid problemId,
        string input,
        string expectedOutput,
        bool isHidden,
        int orderIndex)
    {
        Id = id;
        TournamentProblemId = problemId;
        Input = input;
        ExpectedOutput = expectedOutput;
        IsHidden = isHidden;
        OrderIndex = orderIndex;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static TestCase Create(
        Guid problemId,
        string input,
        string expectedOutput,
        bool isHidden,
        int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input required.");

        if (string.IsNullOrWhiteSpace(expectedOutput))
            throw new ArgumentException("Expected output required.");

        return new TestCase(
            Guid.NewGuid(),
            problemId,
            input,
            expectedOutput,
            isHidden,
            orderIndex);
    }
}
