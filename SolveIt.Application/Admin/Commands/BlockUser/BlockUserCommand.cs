using MediatR;

namespace SolveIt.Application.Admin.BlockUser;

public record BlockUserCommand(Guid UserId) : IRequest;
