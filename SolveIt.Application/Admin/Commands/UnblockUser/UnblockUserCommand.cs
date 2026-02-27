using MediatR;

namespace SolveIt.Application.Admin.UnblockUser;

public sealed record UnblockUserCommand(Guid UserId) : IRequest;
