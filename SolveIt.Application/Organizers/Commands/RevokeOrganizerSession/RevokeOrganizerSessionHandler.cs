using MediatR;
using Solvelt.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeOrganizerSession
{
    public sealed class RevokeOrganizerSessionHandler
        : IRequestHandler<RevokeOrganizerSessionCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public RevokeOrganizerSessionHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenService jwtTokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Unit> Handle(
            RevokeOrganizerSessionCommand request,
            CancellationToken cancellationToken)
        {
            var hashedToken =
                _jwtTokenService.HashRefreshToken(request.RefreshToken);

            var storedToken =
                await _refreshTokenRepository
                    .GetByHashAsync(hashedToken, cancellationToken);

            if (storedToken is not null && !storedToken.IsRevoked)
            {
                storedToken.Revoke();
                await _refreshTokenRepository
                    .SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
