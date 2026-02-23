namespace BuilderScenario.Api.Dtos
{
    public class ScenarioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ActionGroupDto> Groups { get; set; } = new();
    }
}