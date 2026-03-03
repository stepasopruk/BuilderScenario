using BuilderScenario.Core.Entities;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuilderScenario.App.Services
{
    public class ImportServiceClient
    {
        private readonly HttpClient _httpClient;

        public ImportServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Scenario> ImportFromFileAsync(string filePath)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                await using var fileStream = File.OpenRead(filePath);
                var fileName = Path.GetFileName(filePath);

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    GetContentType(filePath));

                content.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync("api/import", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    // Пробуем прочитать как JSON, но если не получается - возвращаем как есть
                    try
                    {
                        // Проверяем, похоже ли на JSON
                        if (error.TrimStart().StartsWith("{"))
                        {
                            var errorObj = JsonSerializer.Deserialize<ErrorResponse>(error);
                            throw new Exception(errorObj?.message ?? errorObj?.error ?? error);
                        }
                        else
                        {
                            // Это просто текст
                            throw new Exception(error);
                        }
                    }
                    catch (JsonException)
                    {
                        // Если не удалось распарсить JSON, выбрасываем исходный текст
                        throw new Exception(error);
                    }
                }

                var importResult = await response.Content.ReadFromJsonAsync<ImportResultDto>();
                return ConvertToScenario(importResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при импорте: {ex.Message}", ex);
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".json" => "application/json",
                ".xml" => "application/xml",
                _ => "application/octet-stream"
            };
        }

        private Scenario ConvertToScenario(ImportResultDto dto)
        {
            if (dto == null) return new Scenario();

            var scenario = new Scenario
            {
                Id = dto.Id,
                Name = dto.Name ?? "Без названия",
                Groups = dto.Groups?.Select(g => new ActionGroup
                {
                    Id = g.Id,
                    Name = g.Name ?? "Группа",
                    Order = g.Order,
                    Steps = g.Steps?.Select(s => new StepItem
                    {
                        Id = s.Id,
                        Name = s.Name ?? "Шаг",
                        Order = s.Order,
                        Actions = s.Actions?.Select(a => new ActionItem
                        {
                            Id = a.Id,
                            Name = a.Name ?? "Действие",
                            Order = a.Order
                        }).ToList() ?? new List<ActionItem>()
                    }).ToList() ?? new List<StepItem>()
                }).ToList() ?? new List<ActionGroup>()
            };

            return scenario;
        }

        public class ImportResultDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public List<ImportGroupDto> Groups { get; set; } = new();
        }

        public class ImportGroupDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Order { get; set; }
            public List<ImportStepDto> Steps { get; set; } = new();
        }

        public class ImportStepDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Order { get; set; }
            public List<ImportActionDto> Actions { get; set; } = new();
        }

        public class ImportActionDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Order { get; set; }
        }

        private class ErrorResponse
        {
            public string message { get; set; }
            public string error { get; set; }
            public string title { get; set; }
        }
    }
}