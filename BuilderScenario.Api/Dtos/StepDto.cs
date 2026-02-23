namespace BuilderScenario.Api.Dtos
{
    public class StepDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ActionDto> Actions { get; set; } = new();
    }
}