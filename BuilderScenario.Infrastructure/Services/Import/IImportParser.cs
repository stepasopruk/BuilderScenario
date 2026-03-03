using BuilderScenario.Core.Entities;

namespace BuilderScenario.Infrastructure.Services.Import
{
    public interface IImportParser
    {
        bool CanParse(string fileName);
        Scenario Parse(string filePath);
    }
}