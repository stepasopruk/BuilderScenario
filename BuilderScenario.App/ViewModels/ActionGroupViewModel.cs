using BuilderScenario.App.Common;
using BuilderScenario.Core.Entities;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace BuilderScenario.App.ViewModels
{
    public class ActionGroupViewModel : BaseViewModel
    {
        public ActionGroup Model { get; }

        public int Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                OnPropertyChanged();
            }
        }

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

        public int Order
        {
            get => Model.Order;
            set
            {
                Model.Order = value;
                OnPropertyChanged();
            }
        }

        public IDropTarget StepDropHandler { get; }

        public ObservableCollection<StepViewModel> Steps { get; set; } = new();

        public RelayCommand AddStepCommand { get; }
        public RelayCommand DeleteStepCommand { get; }

        private readonly CreateScenarioViewModel _parent;

        public ActionGroupViewModel(ActionGroup model, CreateScenarioViewModel parent)
        {
            Model = model;
            _parent = parent;
            StepDropHandler = new StepDropHandler();

            foreach (var step in model.Steps)
                Steps.Add(new StepViewModel(step, _parent));

            AddStepCommand = new RelayCommand(_ => AddStep());
            DeleteStepCommand = new RelayCommand(DeleteStep);
            ValidateName();

            PropertyChanged += (_, __) => _parent.NotifyStateChanged();
            Steps.CollectionChanged += Steps_CollectionChanged;
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

        private void Steps_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Order = i;
            }
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
