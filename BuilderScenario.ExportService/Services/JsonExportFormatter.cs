using System.Text.Encodings.Web;
using System.Text.Json;
using BuilderScenario.Contracts.Export;

namespace BuilderScenario.ExportService.Services
{
    public class JsonExportFormatter : IExportFormatter
    {
        public string ContentType => "application/json";
        public string FileExtension => ".json";

        public string Format(ExportScenarioDto scenario)
        {
            if (scenario == null)
                throw new ArgumentNullException(nameof(scenario));

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(scenario, options);
        }
    }
}