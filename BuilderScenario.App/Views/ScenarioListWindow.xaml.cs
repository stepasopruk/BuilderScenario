using BuilderScenario.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BuilderScenario.App.Views
{
    /// <summary>
    /// Логика взаимодействия для ScenarioListWindow.xaml
    /// </summary>
    public partial class ScenarioListWindow : Window
    {
        public ScenarioListWindow(ScenarioListViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += async (_, __) => await vm.InitializeAsync();
        }
    }
}
