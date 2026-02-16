using BuilderScenario.App.Common;
using BuilderScenario.Application.Interfaces;
using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BuilderScenario.App.ViewModels
{
    public class CreateScenarioViewModel : BaseViewModel
    {
        public RelayCommand DeleteGroupCommand { get; }

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

        public ObservableCollection<ActionGroupViewModel> Groups { get; }
            = new();

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
            Groups.CollectionChanged += (_, __) => RecalculateGroupOrder();
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
                Groups[i].Model.Order = i;

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
            await _repository.SaveAsync(Scenario);
            MessageBox.Show("Сценарий сохранён");
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
