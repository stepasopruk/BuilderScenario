public class ScenarioExportDto
{
    public List<GroupExportDto> Groups { get; set; } = new();
}

public class GroupExportDto
{
    public string GroupData_name { get; set; }
    public string Group_name { get; set; }

    public List<StepExportDto> Steps { get; set; } = new();
}

public class StepExportDto
{
    public string StepData_name { get; set; }
    public string Step_name { get; set; }

    public List<ShellExportDto> Substeps { get; set; } = new();
}

public class ShellExportDto
{
    public string ShellData_name { get; set; }
    public string Shell_name { get; set; }
}