using BuilderScenario.App.ViewModels;
using BuilderScenario.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Infrastructure.Services;

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
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IScenarioService, ScenarioService>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<CreateScenarioViewModel>();
            services.AddTransient<CreateScenarioWindow>();
        }
    }
}
