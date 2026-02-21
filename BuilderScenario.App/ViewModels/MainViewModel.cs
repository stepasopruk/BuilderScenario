using BuilderScenario.App.Common;
using BuilderScenario.App.Views;
using BuilderScenario.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand CreateScenarioCommand { get; }
        public RelayCommand ImportDocxCommand { get; }
        public RelayCommand OpenScenarioListCommand { get; }

        private readonly IServiceProvider _serviceProvider;
        private readonly DocxImportService _docxImportService;
        private readonly ScenarioRepository _repository;
        private readonly IJsonExportService _jsonExportService;

        public MainViewModel(
            IServiceProvider serviceProvider, 
            DocxImportService docxImportService,
            ScenarioRepository repository,
            IJsonExportService jsonExportService)
        {
            _serviceProvider = serviceProvider;
            _docxImportService = docxImportService;
            _repository = repository;
            _jsonExportService = jsonExportService;

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

            ImportDocxCommand = new RelayCommand(_ => ImportDocx());
        }

        private void ImportDocx()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx"
            };

            if (dialog.ShowDialog() != true)
                return;

            var scenario = _docxImportService.Import(dialog.FileName);

            var vm = new CreateScenarioViewModel(_repository, _jsonExportService);
            var window = new CreateScenarioWindow(vm);

            vm.LoadScenario(scenario);

            window.DataContext = vm;
            window.ShowDialog();
        }
    }
}
