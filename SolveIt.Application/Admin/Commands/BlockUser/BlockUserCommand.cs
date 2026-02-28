using MediatR;

namespace SolveIt.Application.Admin.Commands.BlockUser;

public record BlockUserCommand(Guid UserId) : IRequest;
