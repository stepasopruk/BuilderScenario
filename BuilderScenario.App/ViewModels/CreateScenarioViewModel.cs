using BuilderScenario.App.Common;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Services;
using GongSolutions.Wpf.DragDrop;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class CreateScenarioViewModel : BaseViewModel
    {
        public RelayCommand DeleteGroupCommand { get; }

        public ISnackbarMessageQueue SnackbarMessageQueue { get; }
    = new SnackbarMessageQueue();

        public IDropTarget GroupDropHandler { get; } = new GroupDropHandler();

        private ActionGroupViewModel? _selectedGroup;
        public ActionGroupViewModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
            }
        }

        public Scenario Scenario { get; private set; }

        public string ScenarioName
        {
            get => Scenario.Name;
            set
            {
                Scenario.Name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActionGroupViewModel> Groups { get; set; } = new();

        public RelayCommand AddGroupCommand { get; }
        public RelayCommand SaveCommand { get; }

        private readonly ScenarioRepository _repository;

        public CreateScenarioViewModel(ScenarioRepository repository)
        {
            _repository = repository;
            Scenario = new Scenario();

            AddGroupCommand = new RelayCommand(_ => AddGroup());

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());

            DeleteGroupCommand = new RelayCommand(DeleteGroup);

            Groups.CollectionChanged += (_, __) => SaveCommand.RaiseCanExecuteChanged();
            Groups.CollectionChanged += Groups_CollectionChanged;
        }

        public void LoadScenario(Scenario scenario)
        {
            Scenario = scenario;

            Groups.Clear();

            foreach (var group in scenario.Groups)
                Groups.Add(new ActionGroupViewModel(group, this));

            NotifyStateChanged();
        }

        public void NotifyStateChanged()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        public void RecalculateGroupOrder()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                ActionGroupViewModel group = Groups[i];
                group.Model.Order = i;
            }

            NotifyStateChanged();
        }

        private void AddGroup()
        {
            var group = new ActionGroup
            {
                Name = "Новая группа",
                Order = Groups.Count
            };

            Scenario.Groups.Add(group);
            Groups.Add(new ActionGroupViewModel(group, this));
        }

        private async Task SaveAsync()
        {
            var scenario = BuildScenarioFromViewModel();
            await _repository.SaveAsync(scenario);
            SnackbarMessageQueue.Enqueue("Сценарий сохранён");
        }

        private Scenario BuildScenarioFromViewModel()
        {
            var scenario = new Scenario
            {
                Id = Scenario.Id,
                Name = ScenarioName,
                Groups = new List<ActionGroup>()
            };

            for (int i = 0; i < Groups.Count; i++)
            {
                var groupVm = Groups[i];

                var group = new ActionGroup
                {
                    Id = groupVm.Id,
                    Name = groupVm.Name,
                    Order = i,
                    Steps = new List<StepItem>()
                };

                for (int j = 0; j < groupVm.Steps.Count; j++)
                {
                    var stepVm = groupVm.Steps[j];

                    var step = new StepItem
                    {
                        Id = stepVm.Id,
                        Name = stepVm.Name,
                        Order = j,
                        Actions = new List<ActionItem>()
                    };

                    for (int k = 0; k < stepVm.Actions.Count; k++)
                    {
                        var actionVm = stepVm.Actions[k];

                        step.Actions.Add(new ActionItem
                        {
                            Id = actionVm.Id,
                            Name = actionVm.Name,
                            Order = k
                        });
                    }

                    group.Steps.Add(step);
                }

                scenario.Groups.Add(group);
            }

            return scenario;
        }

        private void Groups_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateGroupOrder();
        }

        private void UpdateGroupOrder()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                ActionGroupViewModel group = Groups[i];
                group.Order = i;
            }
        }

        private bool CanSave()
        {
            if (HasErrors)
                return false;

            foreach (var group in Groups)
            {
                if (group.HasErrors)
                    return false;

                foreach (var step in group.Steps)
                {
                    if (step.HasErrors)
                        return false;

                    foreach (var action in step.Actions)
                    {
                        if (action.HasErrors)
                            return false;
                    }
                }
            }

            return true;
        }

        private void DeleteGroup(object parameter)
        {
            if (parameter is not ActionGroupViewModel groupVm)
                return;

            Groups.Remove(groupVm);
            Scenario.Groups.Remove(groupVm.Model);

            RecalculateOrder();
        }

        private void RecalculateOrder()
        {
            for (int i = 0; i < Groups.Count; i++)
                Groups[i].Model.Order = i;
        }
    }
}
