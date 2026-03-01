namespace SolveIt.Api.Contracts;

public sealed class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
