using MediatR;

namespace SolveIt.Application.Auth.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string Name,
    string Email,
    string PhoneNumber
) : IRequest;
