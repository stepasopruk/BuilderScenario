using AutoMapper;
using BuilderScenario.Api.Dtos;
using BuilderScenario.Core.Entities;

namespace BuilderScenario.Api.Mapping
{
    public class ImportMappingProfile : Profile
    {
        public ImportMappingProfile()
        {
            CreateMap<Scenario, ImportResultDto>();
            CreateMap<ActionGroup, ImportGroupDto>();
            CreateMap<StepItem, ImportStepDto>();
            CreateMap<ActionItem, ImportActionDto>();
        }
    }
}
