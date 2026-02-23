namespace BuilderScenario.Api.Dtos
{
    public class CreateScenarioDto
    {
        public string Name { get; set; } = string.Empty;
        public List<CreateActionGroupDto> Groups { get; set; } = new();
    }

    public class CreateActionGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<CreateStepDto> Steps { get; set; } = new();
    }

    public class CreateStepDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<CreateActionDto> Actions { get; set; } = new();
    }

    public class CreateActionDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}