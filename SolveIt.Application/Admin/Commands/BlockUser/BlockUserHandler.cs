using MediatR;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolveIt.Application.Admin.Commands.BlockUser;

namespace SolveIt.Application.Admin.Commands.BlockUser
{
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BlockUserCommandHandler(
            IOrganizerRepository organizerRepository,
            IParticipantRepository participantRepository,
            IUnitOfWork unitOfWork)
        {
            _organizerRepository = organizerRepository;
            _participantRepository = participantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _organizerRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (organizer != null)
            {
                organizer.Block();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var participant = await _participantRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (participant != null)
            {
                participant.Block();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            throw new UserNotFoundException();
        }
    }
}
