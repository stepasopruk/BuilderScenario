using BuilderScenario.App.Common;
using BuilderScenario.Core.Entities;
using System.Collections.ObjectModel;

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
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StepItem> Steps { get; set; } = new();

        public ActionGroupViewModel(ActionGroup model)
        {
            Model = model;
        }
    }
}
