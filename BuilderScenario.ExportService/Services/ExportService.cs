using BuilderScenario.Contracts.Export;

namespace BuilderScenario.ExportService.Services
{
    public class ExportService
    {
        private readonly IEnumerable<IExportFormatter> _formatters;
        private readonly ILogger<ExportService> _logger;

        public ExportService(
           IEnumerable<IExportFormatter> formatters,
           ILogger<ExportService> logger)
        {
            _formatters = formatters;
            _logger = logger;

            // Логируем количество зарегистрированных форматеров
            _logger.LogInformation($"Зарегистрировано форматеров: {_formatters.Count()}");

            foreach (var formatter in _formatters)
            {
                _logger.LogInformation($"Форматер: {formatter.GetType().Name}, расширение: {formatter.FileExtension}");
            }
        }

        public ExportResult Export(ExportScenarioDto scenario, string format = "json")
        {
            // Проверяем, что есть хотя бы один форматер
            if (!_formatters.Any())
            {
                _logger.LogError("Не зарегистрировано ни одного форматера!");
                throw new InvalidOperationException("No export formatters registered. Please register at least one formatter in DI container.");
            }

            // Ищем подходящий форматер
            var formatter = _formatters.FirstOrDefault(f =>
                f.FileExtension.Equals($".{format}", StringComparison.OrdinalIgnoreCase));

            // Если не нашли, берем первый
            if (formatter == null)
            {
                _logger.LogWarning($"Форматер для формата '{format}' не найден. Использую первый доступный: {_formatters.First().GetType().Name}");
                formatter = _formatters.First();
            }

            try
            {
                var content = formatter.Format(scenario);

                return new ExportResult
                {
                    Content = content,
                    ContentType = formatter.ContentType,
                    FileName = $"{SanitizeFileName(scenario.Name)}{formatter.FileExtension}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при форматировании экспорта");
                throw;
            }
        }

        private string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }
    }

    public class ExportResult
    {
        public string Content { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}