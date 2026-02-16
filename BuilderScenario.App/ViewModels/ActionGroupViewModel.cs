using BuilderScenario.App.Common;
using BuilderScenario.Core.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace BuilderScenario.App.ViewModels
{
    public class ActionGroupViewModel : BaseViewModel
    {
        public ActionGroup Model { get; }

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                ValidateName();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StepViewModel> Steps { get; }
            = new();

        public RelayCommand AddStepCommand { get; }
        public RelayCommand DeleteStepCommand { get; }

        private readonly CreateScenarioViewModel _parent;

        public ActionGroupViewModel(ActionGroup model, CreateScenarioViewModel parent)
        {
            Model = model;
            _parent = parent;

            foreach (var step in model.Steps)
                Steps.Add(new StepViewModel(step, _parent));

            AddStepCommand = new RelayCommand(_ => AddStep());
            DeleteStepCommand = new RelayCommand(DeleteStep);
            ValidateName();

            PropertyChanged += (_, __) => _parent.NotifyStateChanged();
            Steps.CollectionChanged += (_, __) => RecalculateStepOrder();
        }

        public void RecalculateStepOrder()
        {
            for (int i = 0; i < Steps.Count; i++)
                Steps[i].Model.Order = i;

            _parent.NotifyStateChanged();
        }

        private void AddStep()
        {
            var step = new StepItem
            {
                Name = "Новый шаг",
                Order = Steps.Count
            };

            Model.Steps.Add(step);
            Steps.Add(new StepViewModel(step, _parent));
        }

        private void DeleteStep(object parameter)
        {
            if (parameter is not StepViewModel stepVm)
                return;

            Steps.Remove(stepVm);
            Model.Steps.Remove(stepVm.Model);

            RecalculateOrder();
        }

        private void RecalculateOrder()
        {
            for (int i = 0; i < Steps.Count; i++)
                Steps[i].Model.Order = i;
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Имя группы не может быть пустым");
            }
        }
    }
}
