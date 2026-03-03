using BuilderScenario.App.Services;
using BuilderScenario.App.ViewModels;
using BuilderScenario.App.Views;
using BuilderScenario.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace BuilderScenario.App
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ScenarioApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5120/");
            });

            // HttpClient для Export микросервиса
            services.AddHttpClient<ExportServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:44332/"); // URL ExportService
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            //services.AddScoped<ExportServiceClient>();

            // HttpClient для Import (через API)
            services.AddHttpClient<ImportServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5120/"); // URL вашего API
                client.Timeout = TimeSpan.FromSeconds(60); // Импорт может быть долгим
            });

            services.AddTransient<MainViewModel>();
            services.AddTransient<CreateScenarioViewModel>();
            services.AddTransient<ScenarioListViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<CreateScenarioWindow>();
            services.AddTransient<ScenarioListWindow>();
        }
    }
}
