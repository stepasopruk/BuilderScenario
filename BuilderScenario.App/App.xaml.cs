using BuilderScenario.App.ViewModels;
using BuilderScenario.App.Views;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

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

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ScenarioApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5120/");
            });

            services.AddSingleton<IScenarioService, ScenarioService>();
            services.AddScoped<IJsonExportService, JsonExportService>();
            services.AddScoped<DocxImportService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<CreateScenarioViewModel>();
            services.AddTransient<ScenarioListViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<CreateScenarioWindow>();
            services.AddTransient<ScenarioListWindow>();
        }
    }
}
