namespace BuilderScenario.Api.Dtos
{
    public class ActionGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<StepDto> Steps { get; set; } = new();
    }
}