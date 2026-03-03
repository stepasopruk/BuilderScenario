namespace BuilderScenario.Api.Dtos
{
    public class ImportResultDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ImportGroupDto> Groups { get; set; } = new();
    }

    public class ImportGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ImportStepDto> Steps { get; set; } = new();
    }

    public class ImportStepDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ImportActionDto> Actions { get; set; } = new();
    }

    public class ImportActionDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
