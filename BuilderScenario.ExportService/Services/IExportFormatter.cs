using BuilderScenario.Contracts.Export;

namespace BuilderScenario.ExportService.Services
{
    public interface IExportFormatter
    {
        string Format(ExportScenarioDto scenario);
        string ContentType { get; }
        string FileExtension { get; }
    }
}