using BuilderScenario.App.Common;
using BuilderScenario.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand CreateScenarioCommand { get; }
        public RelayCommand SelectScenarioCommand { get; }
        public RelayCommand LoadScenarioCommand { get; }
        public RelayCommand OpenScenarioListCommand { get; }

        private readonly IServiceProvider _serviceProvider;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            CreateScenarioCommand = new RelayCommand(_ =>
            {
                var window = _serviceProvider.GetRequiredService<CreateScenarioWindow>();
                window.Show();
            });

            OpenScenarioListCommand = new RelayCommand(_ =>
            {
                var window = ActivatorUtilities.CreateInstance<ScenarioListWindow>(_serviceProvider);
                window.Show();
            });
        }
    }
}
