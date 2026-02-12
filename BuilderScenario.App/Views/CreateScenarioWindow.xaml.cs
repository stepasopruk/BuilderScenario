using BuilderScenario.App.ViewModels;
using System.Windows;

namespace BuilderScenario.App.Views
{
    public partial class CreateScenarioWindow : Window
    {
        public CreateScenarioWindow(CreateScenarioViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
