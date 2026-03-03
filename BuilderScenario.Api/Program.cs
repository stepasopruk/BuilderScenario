using BuilderScenario.Api.Mapping;
using BuilderScenario.Infrastructure.Data;
using BuilderScenario.Infrastructure.Services;
using BuilderScenario.Infrastructure.Services.Import;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BuilderScenario.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigurePipeline(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowDesktopApp", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Database
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "scenarios.db");
            services.AddDbContext<ScenarioDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<ScenarioRepository>();
            services.AddScoped<ImportService>();
            services.AddScoped<IImportParser, DocxImportParser>();

            services.AddHttpClient("ExportService", client =>
            {
                client.BaseAddress = new Uri(config["ExportService:Url"] ?? "http://localhost:44332/");
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddScoped<ExportApiClient>();
            //services.AddAutoMapper(typeof(Program).Assembly);

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ScenarioMappingProfile>();
                cfg.AddProfile<ImportMappingProfile>(); // Добавляем профиль для импорта
            }, typeof(Program).Assembly);
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowDesktopApp");
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => "BuilderScenario API is running...");
        }
    }
}