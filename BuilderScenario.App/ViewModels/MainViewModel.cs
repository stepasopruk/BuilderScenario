using BuilderScenario.App.Common;
using BuilderScenario.App.Services;
using BuilderScenario.App.Views;
using BuilderScenario.Infrastructure.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand CreateScenarioCommand { get; }
        public RelayCommand ImportDocxCommand { get; }
        public RelayCommand OpenScenarioListCommand { get; }

        private readonly IServiceProvider _serviceProvider;
        private readonly ScenarioApiClient _apiClient;
        private readonly ImportServiceClient _importClient;
        private readonly ExportServiceClient _exportClient;

        public MainViewModel(
            IServiceProvider serviceProvider,
            ScenarioApiClient apiClient,
            ImportServiceClient importClient,
            ExportServiceClient exportClient)
        {
            _serviceProvider = serviceProvider;
            _exportClient = exportClient;
            _apiClient = apiClient;
            _importClient = importClient;

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

            ImportDocxCommand = new RelayCommand(async _ => await ImportAsync());
        }

        // Метод для импорта
        private async Task ImportAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Все поддерживаемые форматы (*.docx;*.json;*.xml)|*.docx;*.json;*.xml|" +
                        "Word документы (*.docx)|*.docx|" +
                        "JSON файлы (*.json)|*.json|" +
                        "XML файлы (*.xml)|*.xml",
                Multiselect = false,
                Title = "Выберите файл для импорта"
            };

            if (dialog.ShowDialog() != true)
                return;

            SnackbarMessageQueue snackbarMessageQueue = new SnackbarMessageQueue();

            try
            {
                var importedScenario = await _importClient.ImportFromFileAsync(dialog.FileName);
                var vm = new CreateScenarioViewModel(_apiClient, _exportClient);
                var window = new CreateScenarioWindow(vm);
                vm.LoadScenario(importedScenario);

                window.DataContext = vm;
                window.ShowDialog();

                snackbarMessageQueue.Enqueue("Сценарий успешно импортирован");
            }
            catch (Exception ex)
            {
                snackbarMessageQueue.Enqueue($"Ошибка импорта: {ex.Message}");

                // Показываем детали в отдельном окне
                MessageBox.Show($"Детали ошибки:\n{ex}", "Ошибка импорта",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
