using BuilderScenario.App.Common;
using BuilderScenario.App.Views;
using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class ScenarioListViewModel : BaseViewModel
    {
        private readonly ScenarioRepository _repository;
        private readonly IServiceProvider _provider;

        public ObservableCollection<Scenario> Scenarios { get; }
            = new();

        public RelayCommand RefreshCommand { get; }
        public RelayCommand OpenCommand { get; }

        public ScenarioListViewModel(
            ScenarioRepository repository,
            IServiceProvider provider)
        {
            _repository = repository;
            _provider = provider;

            RefreshCommand = new RelayCommand(async _ => await LoadAsync());
            OpenCommand = new RelayCommand(async s => await OpenAsync(s));
        }

        public async Task InitializeAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            Scenarios.Clear();

            var list = await _repository.GetAllAsync();

            foreach (var scenario in list)
                Scenarios.Add(scenario);

            MessageBox.Show($"Найдено сценариев: {list.Count}");
        }

        private async Task OpenAsync(object parameter)
        {
            if (parameter is not Scenario scenario)
                return;

            var fullScenario = await _repository.GetByIdAsync(scenario.Id);
            if (fullScenario == null)
                return;

            var vm = _provider.GetRequiredService<CreateScenarioViewModel>();
            vm.LoadScenario(fullScenario);

            var window = new CreateScenarioWindow(vm);
            window.Show();
        }
    }
}
