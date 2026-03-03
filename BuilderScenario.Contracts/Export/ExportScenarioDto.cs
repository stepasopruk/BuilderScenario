namespace BuilderScenario.Contracts.Export
{
    public class ExportScenarioDto
    {
        public string Name { get; set; } = string.Empty;
        public List<ExportGroupDto> Groups { get; set; } = new();
    }

    public class ExportGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ExportStepDto> Steps { get; set; } = new();
    }

    public class ExportStepDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ExportActionDto> Actions { get; set; } = new();
    }

    public class ExportActionDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}