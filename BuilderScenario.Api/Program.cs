
using AutoMapper;
using BuilderScenario.Api.Mapping;
using BuilderScenario.Infrastructure.Data;
using BuilderScenario.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BuilderScenario.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ScenarioMappingProfile>();
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Решение проблемы с циклическими ссылками
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Сохраняем имена свойств как есть
                });

            // Добавляем CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowDesktopApp", policy =>
                {
                    policy.AllowAnyOrigin()      // Разрешаем любой источник (для development)
                          .AllowAnyMethod()      // Разрешаем любые HTTP методы
                          .AllowAnyHeader();     // Разрешаем любые заголовки
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "scenarios.db");
            builder.Services.AddDbContext<ScenarioDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddScoped<ScenarioRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ВАЖНО: Порядок middleware имеет значение!
            app.UseCors("AllowDesktopApp");  // CORS должен быть до Authorization и MapControllers

            app.UseAuthorization();

            app.MapControllers();

            // Добавляем простую страницу приветствия для проверки
            app.MapGet("/", () => "BuilderScenario API is running...");

            app.Run();
        }
    }
}
