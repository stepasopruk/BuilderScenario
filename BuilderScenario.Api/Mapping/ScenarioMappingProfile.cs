using AutoMapper;
using BuilderScenario.Core.Entities;
using BuilderScenario.Api.Dtos;

namespace BuilderScenario.Api.Mapping
{
    public class ScenarioMappingProfile : Profile
    {
        public ScenarioMappingProfile()
        {
            // Маппинг из Entity в DTO
            CreateMap<Scenario, ScenarioDto>();
            CreateMap<ActionGroup, ActionGroupDto>();
            CreateMap<StepItem, StepDto>();
            CreateMap<ActionItem, ActionDto>();

            // Маппинг из Create DTO в Entity
            CreateMap<CreateScenarioDto, Scenario>();
            CreateMap<CreateActionGroupDto, ActionGroup>();
            CreateMap<CreateStepDto, StepItem>();
            CreateMap<CreateActionDto, ActionItem>();
        }
    }
}