using MediatR;
using SolveIt.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Organizers.Commands.RevokeOrganizerSession
{
    // LEGACY AUTH HANDLER
    // Replaced by unified /api/auth flow.
    // Safe to remove after full migration validation.
    public sealed class RevokeOrganizerSessionHandler
        : IRequestHandler<RevokeOrganizerSessionCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RevokeOrganizerSessionHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            RevokeOrganizerSessionCommand request,
            CancellationToken cancellationToken)
        {
            // Guard clause (fail silently)
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Unit.Value;

            var hashedToken =
                _jwtTokenService.HashRefreshToken(request.RefreshToken);

            var storedToken =
                await _refreshTokenRepository
                    .GetByHashAsync(hashedToken, cancellationToken);

            if (storedToken is not null && !storedToken.IsRevoked)
            {
                storedToken.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Unit.Value;
        }
    }
}
