namespace SolveIt.Api.Contracts
{
    public sealed class RegisterOrganizerRequestDto
    {
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string ConfirmPassword { get; init; } = null!;
    }
}
