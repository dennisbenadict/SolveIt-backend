using MediatR;
using SolveIt.Application.Admin.UnblockUser;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Admin.Commands.UnblockUser
{
    public sealed class UnblockUserCommandHandler
        : IRequestHandler<UnblockUserCommand>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnblockUserCommandHandler(
            IOrganizerRepository organizerRepository,
            IParticipantRepository participantRepository,
            IUnitOfWork unitOfWork)
        {
            _organizerRepository = organizerRepository;
            _participantRepository = participantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            UnblockUserCommand request,
            CancellationToken cancellationToken)
        {
            var organizer = await _organizerRepository
                .GetByIdAsync(request.UserId, cancellationToken);

            if (organizer is not null)
            {
                organizer.Unblock();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var participant = await _participantRepository
                .GetByIdAsync(request.UserId, cancellationToken);

            if (participant is not null)
            {
                participant.Unblock();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            throw new UserNotFoundException();
        }
    }
}
