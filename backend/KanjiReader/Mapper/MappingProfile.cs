using AutoMapper;
using KanjiReader.Domain.DomainObjects;
using KanjiReader.Infrastructure.Database.Models;
using KanjiReader.Presentation.Dtos.Texts;

namespace KanjiReader.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProcessingResultDb, ProcessingResult>();
        CreateMap<ProcessingResult, ProcessingResultDb>();
        
        CreateMap<EventDb, Event>();
        CreateMap<Event, EventDb>();
        
        CreateMap<KanjiDb, Kanji>();
        CreateMap<Kanji, KanjiDb>();
        
        CreateMap<ProcessingResult, ProcessingResultDto>();
    }
}