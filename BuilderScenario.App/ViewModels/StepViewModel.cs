using BuilderScenario.App.Common;
using BuilderScenario.Core.Entities;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace BuilderScenario.App.ViewModels
{
    public class StepViewModel : BaseViewModel
    {
        public StepItem Model { get; }

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

        public IDropTarget ActionDropHandler { get; }
        public ObservableCollection<ActionViewModel> Actions { get; set; } = new();

        public RelayCommand AddActionCommand { get; }
        public RelayCommand DeleteActionCommand { get; }

        private readonly CreateScenarioViewModel _parent;

        public StepViewModel(StepItem model, CreateScenarioViewModel parent)
        {
            Model = model;
            _parent = parent;
            ActionDropHandler = new ActionDropHandler();

            foreach (var action in model.Actions)
                Actions.Add(new ActionViewModel(action, _parent));

            AddActionCommand = new RelayCommand(_ => AddAction());
            DeleteActionCommand = new RelayCommand(DeleteAction);
            ValidateName();

            PropertyChanged += (_, __) => _parent.NotifyStateChanged();
            Actions.CollectionChanged += Actions_CollectionChanged;
        }

        private void Actions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Order = i;
            }
        }

        private void AddAction()
        {
            var action = new ActionItem
            {
                Name = "Новое действие",
                Order = Actions.Count
            };

            Model.Actions.Add(action);
            Actions.Add(new ActionViewModel(action, _parent));
        }

        private void DeleteAction(object parameter)
        {
            if (parameter is not ActionViewModel actionVm)
                return;

            Actions.Remove(actionVm);
            Model.Actions.Remove(actionVm.Model);

            RecalculateOrder();
        }

        private void RecalculateOrder()
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Order = i;
            }
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Имя шага не может быть пустым");
            }
        }
    }
}
