using BuilderScenario.App.ViewModels;
using BuilderScenario.App.Views;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Infrastructure.Data;
using BuilderScenario.Infrastructure.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace BuilderScenario.App
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ScenarioDbContext>();
            db.Database.EnsureCreated();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BuilderScenario",
                "scenarios.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IScenarioService, ScenarioService>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<CreateScenarioViewModel>();
            services.AddTransient<CreateScenarioWindow>();
            services.AddDbContext<ScenarioDbContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));
            services.AddScoped<ScenarioRepository>();
            services.AddTransient<ScenarioListViewModel>();
            services.AddTransient<ScenarioListWindow>();
            services.AddScoped<IJsonExportService, JsonExportService>();
            services.AddScoped<DocxImportService>();
        }
    }
}
