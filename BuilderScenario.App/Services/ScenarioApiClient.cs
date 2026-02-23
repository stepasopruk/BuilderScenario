using BuilderScenario.Core.Entities;
using System.Net.Http;
using System.Net.Http.Json;

public class ScenarioApiClient
{
    private readonly HttpClient _httpClient;

    public ScenarioApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Scenario>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Scenario>>("api/scenarios")
               ?? new List<Scenario>();
    }

    public async Task<Scenario?> GetAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Scenario>($"api/scenarios/{id}");
    }

    public async Task CreateAsync(Scenario scenario)
    {
        var response = await _httpClient.PostAsJsonAsync("api/scenarios", scenario);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(Scenario scenario)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/scenarios/{scenario.Id}", scenario);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/scenarios/{id}");
        response.EnsureSuccessStatusCode();
    }
}