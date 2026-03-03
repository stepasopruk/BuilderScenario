using BuilderScenario.Contracts.Export;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BuilderScenario.ExportService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly Services.ExportService _exportService;
        private readonly ILogger<ExportController> _logger;

        public ExportController(
            Services.ExportService exportService,
            ILogger<ExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Export(
        [FromBody] ExportScenarioDto scenario,
        [FromQuery] string format = "json")
        {
            try
            {
                _logger.LogInformation($"Получен запрос на экспорт сценария: {scenario?.Name}, формат: {format}");

                if (scenario == null)
                    return BadRequest("Scenario is required");

                var result = _exportService.Export(scenario, format);

                var bytes = Encoding.UTF8.GetBytes(result.Content);
                return File(bytes, result.ContentType, result.FileName);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No export formatters registered"))
            {
                _logger.LogError(ex, "Ошибка конфигурации сервера");
                return StatusCode(500, "Server configuration error: No export formatters registered");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting scenario: {ScenarioName}", scenario?.Name);
                return BadRequest(new { error = "Export failed", message = ex.Message });
            }
        }

        [HttpGet("formats")]
        public IActionResult GetSupportedFormats()
        {
            return Ok(new[] { "json" });
        }
    }
}