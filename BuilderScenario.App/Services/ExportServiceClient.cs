using BuilderScenario.Core.Entities;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuilderScenario.App.Services
{
    public class ExportServiceClient
    {
        private readonly HttpClient _httpClient;

        public ExportServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ExportToFileAsync(Scenario scenario, string filePath, string format = "json")
        {
            try
            {
                // Создаем DTO для экспорта (без ID и навигационных свойств)
                var exportDto = new
                {
                    Name = scenario.Name,
                    Groups = scenario.Groups.Select(g => new
                    {
                        Name = g.Name,
                        Order = g.Order,
                        Steps = g.Steps.Select(s => new
                        {
                            Name = s.Name,
                            Order = s.Order,
                            Actions = s.Actions.Select(a => new
                            {
                                Name = a.Name,
                                Order = a.Order
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };

                // Отправляем на микросервис
                var response = await _httpClient.PostAsJsonAsync($"api/export?format={format}", exportDto);
                response.EnsureSuccessStatusCode();

                // Получаем файл
                var bytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(filePath, bytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при экспорте: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> ExportAsync(Scenario scenario, string format = "json")
        {
            var exportDto = new
            {
                Name = scenario.Name,
                Groups = scenario.Groups.Select(g => new
                {
                    Name = g.Name,
                    Order = g.Order,
                    Steps = g.Steps.Select(s => new
                    {
                        Name = s.Name,
                        Order = s.Order,
                        Actions = s.Actions.Select(a => new
                        {
                            Name = a.Name,
                            Order = a.Order
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync($"api/export?format={format}", exportDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}