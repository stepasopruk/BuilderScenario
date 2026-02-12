using BuilderScenario.App.Common;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Core.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuilderScenario.App.ViewModels
{
    public class CreateScenarioViewModel : BaseViewModel
    {
        private readonly IScenarioService _scenarioService;

        public Scenario Scenario { get; } = new();

        public string ScenarioName
        {
            get => Scenario.Name;
            set
            {
                Scenario.Name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActionGroupViewModel> Groups { get; }
            = new();

        public RelayCommand AddGroupCommand { get; }
        public RelayCommand SaveCommand { get; }

        public CreateScenarioViewModel(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;

            AddGroupCommand = new RelayCommand(_ => AddGroup());

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
        }

        private void AddGroup()
        {
            var group = new ActionGroup
            {
                Name = "Новая группа",
                Order = Groups.Count
            };

            Scenario.Groups.Add(group);
            Groups.Add(new ActionGroupViewModel(group));
        }

        private async Task SaveAsync()
        {
            Scenario.Groups = Groups.Select(g => g.Model).ToList();
            await _scenarioService.SaveAsync(Scenario);
        }
    }
}
