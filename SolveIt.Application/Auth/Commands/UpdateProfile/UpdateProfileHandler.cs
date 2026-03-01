using MediatR;
using SolveIt.Application.Common.Exceptions;
using SolveIt.Application.Common.Interfaces;
using SolveIt.Application.Interfaces;
using SolveIt.Application.Organizers.Exceptions;
using SolveIt.Domain.Exceptions;

namespace SolveIt.Application.Auth.Commands.UpdateProfile;

public sealed class UpdateProfileHandler
    : IRequestHandler<UpdateProfileCommand>
{
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(
        IOrganizerRepository organizerRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork)
    {
        _organizerRepository = organizerRepository;
        _participantRepository = participantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existsInOrganizers =
            await _organizerRepository
                .ExistsByEmailAsync(normalizedEmail, cancellationToken);

        var existsInParticipants =
            await _participantRepository
                .ExistsByEmailAsync(normalizedEmail, cancellationToken);

        var organizer = await _organizerRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (organizer is not null)
        {
            if ((existsInOrganizers || existsInParticipants) &&
                organizer.Email != normalizedEmail)
            {
                throw new EmailAlreadyExistsException();
            }

            organizer.UpdateProfile(
                request.Name,
                normalizedEmail,
                request.PhoneNumber);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var participant = await _participantRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (participant is null)
            throw new UserNotFoundException();

        if ((existsInOrganizers || existsInParticipants) &&
            participant.Email != normalizedEmail)
        {
            throw new EmailAlreadyExistsException();
        }

        participant.UpdateProfile(
            request.Name,
            normalizedEmail,
            request.PhoneNumber);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
