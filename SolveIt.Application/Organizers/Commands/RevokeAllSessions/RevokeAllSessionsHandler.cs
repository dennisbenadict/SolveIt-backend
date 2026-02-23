using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeAllSessions
{
    public sealed class RevokeAllSessionsHandler
        : IRequestHandler<RevokeAllSessionsCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RevokeAllSessionsHandler(
            IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Unit> Handle(
            RevokeAllSessionsCommand request,
            CancellationToken cancellationToken)
        {
            if (request.OrganizerId == Guid.Empty)
                throw new InvalidCredentialsException();

            await _refreshTokenRepository
                .RevokeAllByOrganizerIdAsync(
                    request.OrganizerId,
                    cancellationToken);

            await _refreshTokenRepository
                .SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
