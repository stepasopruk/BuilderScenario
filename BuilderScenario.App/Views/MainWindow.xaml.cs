using BuilderScenario.App.ViewModels;
using System.Windows;

namespace BuilderScenario.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
