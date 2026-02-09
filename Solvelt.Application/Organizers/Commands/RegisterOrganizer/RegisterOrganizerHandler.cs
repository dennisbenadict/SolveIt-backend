using MediatR;
using Solvelt.Application.Interfaces;
using Solvelt.Domain.Organizers;

namespace Solvelt.Application.Organizers.Commands.RegisterOrganizer;

public sealed class RegisterOrganizerHandler
    : IRequestHandler<RegisterOrganizerCommand, Guid>
{
    private readonly IOrganizerRepository _organizerRepository;

    public RegisterOrganizerHandler(IOrganizerRepository organizerRepository)
    {
        _organizerRepository = organizerRepository;
    }

    public async Task<Guid> Handle(
        RegisterOrganizerCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _organizerRepository
            .ExistsByEmailAsync(request.Email, cancellationToken);

        if (exists)
            throw new InvalidOperationException("Organizer already exists");

        var organizer = Organizer.Create(
            request.Name,
            request.Email,
            authProvider: "local"
        );

        await _organizerRepository.AddAsync(organizer, cancellationToken);

        return organizer.Id;
    }
}

