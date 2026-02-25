using BuilderScenario.Api.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BuilderScenario.Api.Tests
{
    // Класс с тестами, использует фабрику и реализует IAsyncLifetime
    public class ScenarioControllerTests
        : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;           // HTTP клиент для запросов к API
        private readonly CustomWebApplicationFactory<Program> _factory; // Фабрика

        // Конструктор - вызывается перед каждым тестом
        public ScenarioControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(); // Создаем HTTP клиент для тестового API
        }

        // IAsyncLifetime.InitializeAsync - вызывается перед каждым тестом
        public async ValueTask InitializeAsync()
        {
            _factory.ClearDatabase(); // Очищаем базу данных перед каждым тестом
            await Task.CompletedTask;
        }

        // IAsyncLifetime.DisposeAsync - вызывается после каждого теста
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        // ТЕСТ 1: Проверка GET /api/scenarios когда нет данных
        [Fact] // Атрибут, указывающий что это тестовый метод
        public async Task GET_ShouldReturn_EmptyList_WhenNoData()
        {
            // Act - выполняем действие
            var response = await _client.GetAsync("/api/scenarios");

            // Assert - проверяем результат
            response.StatusCode.Should().Be(HttpStatusCode.OK); // Должен быть статус 200

            var scenarios = await response.Content
                .ReadFromJsonAsync<List<ScenarioDto>>(); // Парсим JSON в список DTO

            scenarios.Should().NotBeNull();  // Список не должен быть null
            scenarios.Should().BeEmpty();    // Список должен быть пустым
        }

        // ТЕСТ 2: Проверка POST и GET
        [Fact]
        public async Task POST_ShouldCreateScenario_AndReturnItInGet()
        {
            // Arrange - готовим данные для теста
            var newScenario = new CreateScenarioDto  // DTO для создания
            {
                Name = "Test Scenario",
                Groups = new List<CreateActionGroupDto>
                {
                    new CreateActionGroupDto
                    {
                        Name = "GroupData_1",
                        Steps = new List<CreateStepDto>
                        {
                            new CreateStepDto
                            {
                                Name = "StepData_1_1",
                                Actions = new List<CreateActionDto>
                                {
                                    new CreateActionDto
                                    {
                                        Name = "ShellData_1_1_1"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act 1 - POST запрос на создание
            var postResponse = await _client.PostAsJsonAsync("/api/scenarios", newScenario);

            // Assert 1 - проверяем ответ на POST
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created); // Должен быть 201

            // Читаем созданный сценарий из ответа
            var createdScenario = await postResponse.Content.ReadFromJsonAsync<ScenarioDto>();
            createdScenario.Should().NotBeNull(); // Не null
            createdScenario!.Id.Should().BeGreaterThan(0); // ID должен быть > 0 (успешно сохранен)
            createdScenario.Name.Should().Be("Test Scenario"); // Имя сохранилось

            // Act 2 - GET запрос всех сценариев
            var getResponse = await _client.GetAsync("/api/scenarios");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Статус 200

            var scenarios = await getResponse.Content
                .ReadFromJsonAsync<List<ScenarioDto>>(); // Парсим список

            // Assert 2 - проверяем что сценарий действительно создан
            scenarios.Should().NotBeNull(); // Список не null
            scenarios.Should().ContainSingle(); // Должен быть ровно 1 элемент

            var created = scenarios!.First(); // Берем первый (и единственный)
            created.Name.Should().Be("Test Scenario"); // Имя совпадает
            created.Id.Should().Be(createdScenario.Id); // ID совпадает

            // Проверяем всю иерархию данных
            created.Groups.Should().HaveCount(1); // 1 группа
            created.Groups.First().Name.Should().Be("GroupData_1"); // Имя группы
            created.Groups.First().Steps.Should().HaveCount(1); // 1 шаг
            created.Groups.First().Steps.First().Name.Should().Be("StepData_1_1"); // Имя шага
            created.Groups.First().Steps.First().Actions.Should().HaveCount(1); // 1 действие
            created.Groups.First().Steps.First().Actions.First().Name.Should().Be("ShellData_1_1_1"); // Имя действия
        }

        // ТЕСТ 3: Проверка PUT (обновление)
        [Fact]
        public async Task PUT_ShouldUpdateScenario()
        {
            // Arrange - создаем сценарий для обновления
            var createDto = new CreateScenarioDto
            {
                Name = "Test Scenario",
                Groups = new List<CreateActionGroupDto>() // Пустые группы
            };

            // POST запрос на создание
            var postResponse = await _client.PostAsJsonAsync("/api/scenarios", createDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created); // Проверяем создание

            var created = await postResponse.Content.ReadFromJsonAsync<ScenarioDto>();
            created.Should().NotBeNull();

            // Act - обновляем сценарий
            var updateDto = new CreateScenarioDto
            {
                Name = "Updated Name", // Новое имя
                Groups = new List<CreateActionGroupDto>()
            };

            // PUT запрос с новыми данными
            var putResponse = await _client.PutAsJsonAsync(
                $"/api/scenarios/{created!.Id}", // URL с ID сценария
                updateDto);

            putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent); // Должен быть 204

            // Assert - проверяем что обновление применилось
            var getResponse = await _client.GetAsync($"/api/scenarios/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // 200 OK

            var updated = await getResponse.Content.ReadFromJsonAsync<ScenarioDto>();
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Name"); // Имя должно измениться
        }

        // ТЕСТ 4: Проверка DELETE
        [Fact]
        public async Task DELETE_ShouldRemoveScenario()
        {
            // Arrange - создаем сценарий для удаления
            var createDto = new CreateScenarioDto
            {
                Name = "To Delete",
                Groups = new List<CreateActionGroupDto>()
            };

            var postResponse = await _client.PostAsJsonAsync("/api/scenarios", createDto);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await postResponse.Content.ReadFromJsonAsync<ScenarioDto>();
            created.Should().NotBeNull();

            // Act - удаляем сценарий
            var deleteResponse = await _client.DeleteAsync($"/api/scenarios/{created!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent); // Должен быть 204

            // Assert - проверяем что сценарий удален
            // Запрос по ID должен вернуть 404
            var getResponse = await _client.GetAsync($"/api/scenarios/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound); // 404 Not Found

            // Проверяем что список вообще пуст
            var getAllResponse = await _client.GetAsync("/api/scenarios");
            var allScenarios = await getAllResponse.Content.ReadFromJsonAsync<List<ScenarioDto>>();
            allScenarios.Should().BeEmpty(); // Должен быть пустым
        }

        // ТЕСТ 5: Проверка GET по несуществующему ID
        [Fact]
        public async Task GET_ById_ShouldReturnNotFound_WhenScenarioDoesNotExist()
        {
            // Act - запрос с несуществующим ID
            var response = await _client.GetAsync("/api/scenarios/999");

            // Assert - должен быть 404
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ТЕСТ 6: Проверка что несколько запросов используют одну БД
        [Fact]
        public async Task MultipleRequests_ShouldUseSameDatabase()
        {
            // Создаем первый сценарий
            var scenario1 = new CreateScenarioDto
            {
                Name = "Scenario 1",
                Groups = new List<CreateActionGroupDto>()
            };
            var response1 = await _client.PostAsJsonAsync("/api/scenarios", scenario1);
            response1.StatusCode.Should().Be(HttpStatusCode.Created); // Проверяем создание

            // Создаем второй сценарий
            var scenario2 = new CreateScenarioDto
            {
                Name = "Scenario 2",
                Groups = new List<CreateActionGroupDto>()
            };
            var response2 = await _client.PostAsJsonAsync("/api/scenarios", scenario2);
            response2.StatusCode.Should().Be(HttpStatusCode.Created); // Проверяем создание

            // Получаем все сценарии
            var getResponse = await _client.GetAsync("/api/scenarios");
            var scenarios = await getResponse.Content.ReadFromJsonAsync<List<ScenarioDto>>();

            // Должно быть 2 сценария (значит использовалась одна БД)
            scenarios.Should().HaveCount(2);
            // Имена должны содержать оба созданных
            scenarios.Select(s => s.Name).Should().Contain(new[] { "Scenario 1", "Scenario 2" });
        }
    }
}