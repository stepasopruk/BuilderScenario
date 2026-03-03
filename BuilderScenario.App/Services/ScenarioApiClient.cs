using BuilderScenario.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BuilderScenario.App.Services
{
    public class ScenarioApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScenarioApiClient> _logger;

        public ScenarioApiClient(HttpClient httpClient, ILogger<ScenarioApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Scenario>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Scenario>>("api/scenarios")
                    ?? new List<Scenario>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting scenarios");
                throw new ServiceException("Failed to load scenarios", ex);
            }
        }

        public async Task<Scenario?> GetAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Scenario>($"api/scenarios/{id}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting scenario {Id}", id);
                throw new ServiceException($"Failed to load scenario {id}", ex);
            }
        }

        public async Task<Scenario> CreateAsync(Scenario scenario)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/scenarios", scenario);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Scenario>()
                    ?? throw new ServiceException("Invalid response from server");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error creating scenario");
                throw new ServiceException("Failed to create scenario", ex);
            }
        }

        public async Task UpdateAsync(Scenario scenario)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/scenarios/{scenario.Id}", scenario);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error updating scenario {Id}", scenario.Id);
                throw new ServiceException($"Failed to update scenario {scenario.Id}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/scenarios/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error deleting scenario {Id}", id);
                throw new ServiceException($"Failed to delete scenario {id}", ex);
            }
        }
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}