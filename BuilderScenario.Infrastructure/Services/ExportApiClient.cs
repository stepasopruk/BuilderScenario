using BuilderScenario.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BuilderScenario.Infrastructure.Services
{
    public class ExportApiClient
    {
        private readonly HttpClient _httpClient;

        public ExportApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExportToJsonAsync(Scenario scenario)
        {
            var response = await _httpClient.PostAsJsonAsync("api/export", scenario);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
