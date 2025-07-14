using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProcessingResultDb, ProcessingResult>();
        CreateMap<EventDb, Event>();
    }
}