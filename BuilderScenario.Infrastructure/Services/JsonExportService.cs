using System.Text.Json;
using BuilderScenario.Core.Entities;
using System.Text.Encodings.Web;


public interface IJsonExportService
{
    Task ExportAsync(Scenario scenario, string filePath);
}

public class JsonExportService : IJsonExportService
{
    public async Task ExportAsync(Scenario scenario, string filePath)
    {
        var exportDto = BuildExportModel(scenario);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(exportDto, options);

        await File.WriteAllTextAsync(filePath, json);
    }

    private ScenarioExportDto BuildExportModel(Scenario scenario)
    {
        var dto = new ScenarioExportDto();

        for (int i = 0; i < scenario.Groups.Count; i++)
        {
            var group = scenario.Groups[i];

            var groupDto = new GroupExportDto
            {
                GroupData_name = $"GroupData_{i + 1}",
                Group_name = group.Name
            };

            for (int j = 0; j < group.Steps.Count; j++)
            {
                var step = group.Steps[j];

                var stepDto = new StepExportDto
                {
                    StepData_name = $"StepData_{i + 1}_{j + 1}",
                    Step_name = step.Name
                };

                for (int k = 0; k < step.Actions.Count; k++)
                {
                    var action = step.Actions[k];

                    var shellDto = new ShellExportDto
                    {
                        ShellData_name = $"ShellData_{i + 1}_{j + 1}_{k + 1}",
                        Shell_name = action.Name
                    };

                    stepDto.Substeps.Add(shellDto);
                }

                groupDto.Steps.Add(stepDto);
            }

            dto.Groups.Add(groupDto);
        }

        return dto;
    }
}