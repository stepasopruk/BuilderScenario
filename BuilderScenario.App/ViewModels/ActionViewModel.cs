using BuilderScenario.App.Common;
using BuilderScenario.Core.Entities;

namespace BuilderScenario.App.ViewModels
{
    public class ActionViewModel : BaseViewModel
    {
        public ActionItem Model { get; }

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

        private readonly CreateScenarioViewModel _parent;

        public ActionViewModel(ActionItem model, CreateScenarioViewModel parent)
        {
            Model = model;
            _parent = parent;

            ValidateName();

            PropertyChanged += (_, __) => _parent.NotifyStateChanged();
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Имя действия не может быть пустым");
            }
        }
    }
}
