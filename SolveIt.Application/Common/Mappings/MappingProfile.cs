using AutoMapper;
using SolveIt.Application.Common.DTOs.OrganizerAuthDTOs;
using Solvelt.Domain.Organizers;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Organizer, OrganizerProfileDto>();
    }
}
 
