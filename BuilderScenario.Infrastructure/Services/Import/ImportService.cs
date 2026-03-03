using BuilderScenario.Core.Entities;

namespace BuilderScenario.Infrastructure.Services.Import
{
    public class ImportService
    {
        private readonly IEnumerable<IImportParser> _parsers;

        public ImportService(IEnumerable<IImportParser> parsers)
        {
            _parsers = parsers;
        }

        public Scenario Import(string filePath, string originalFileName = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // Используем оригинальное имя файла для определения парсера, если оно передано
            var fileNameForParsing = originalFileName ?? filePath;

            var parser = _parsers.FirstOrDefault(p => p.CanParse(fileNameForParsing));

            if (parser == null)
                throw new NotSupportedException($"Unsupported file format: {Path.GetExtension(fileNameForParsing)}");

            return parser.Parse(filePath);
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return _parsers.Select(p =>
            {
                // Простой способ получить расширение из имени класса
                var parserName = p.GetType().Name;
                if (parserName.Contains("Docx")) return ".docx";
                if (parserName.Contains("Json")) return ".json";
                if (parserName.Contains("Xml")) return ".xml";
                return ".unknown";
            });
        }
    }
}