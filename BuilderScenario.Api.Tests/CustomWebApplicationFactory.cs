using BuilderScenario.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BuilderScenario.Api.Tests
{
    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        // SQLite соединение, которое будет жить пока живет фабрика
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            // Создаем и открываем SQLite in-memory соединение
            // DataSource=:memory: означает, что база будет в оперативной памяти
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // Открываем соединение
        }

        // Этот метод переопределяет конфигурацию веб-хоста для тестов
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Ищем регистрацию DbContextOptions<ScenarioDbContext> в сервисах
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ScenarioDbContext>));

                // Если нашли - удаляем (чтобы заменить на тестовую БД)
                if (descriptor != null)
                    services.Remove(descriptor);

                // Добавляем SQLite in-memory контекст с нашим открытым соединением
                services.AddDbContext<ScenarioDbContext>(options =>
                {
                    options.UseSqlite(_connection); // Используем одно соединение для всех запросов
                }, ServiceLifetime.Scoped); // Scoped - новый экземпляр на каждый HTTP запрос

                // Создаем временный сервис провайдер для инициализации БД
                var serviceProvider = services.BuildServiceProvider();

                // Создаем scope и инициализируем базу данных
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ScenarioDbContext>();
                    db.Database.EnsureCreated(); // Создаем таблицы, если их нет
                }
            });
        }

        // Метод для очистки базы данных перед каждым тестом
        public void ClearDatabase()
        {
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ScenarioDbContext>();

                // Удаляем все данные из таблиц в правильном порядке (из-за внешних ключей)
                db.Database.ExecuteSqlRaw("DELETE FROM Actions");      // Сначала действия
                db.Database.ExecuteSqlRaw("DELETE FROM Steps");        // Потом шаги
                db.Database.ExecuteSqlRaw("DELETE FROM ActionGroups"); // Потом группы
                db.Database.ExecuteSqlRaw("DELETE FROM Scenarios");    // Потом сценарии

                // Сбрасываем счетчики автоинкремента
                db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence");
            }
        }

        // Освобождаем ресурсы при уничтожении фабрики
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Close();   // Закрываем соединение
                _connection?.Dispose(); // Освобождаем ресурсы
            }
        }
    }
}