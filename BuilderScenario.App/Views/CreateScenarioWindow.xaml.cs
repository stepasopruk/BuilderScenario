using BuilderScenario.App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BuilderScenario.App.Views
{
    public partial class CreateScenarioWindow : Window
    {
        private bool _treeVisible = true;
        private GridLength _wasTreeWidth = new GridLength(0);

        public CreateScenarioWindow(CreateScenarioViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Добавляем обработчик для перетаскивания
            this.PreviewDragOver += (s, e) =>
            {
                Point mousePos = e.GetPosition(MainScrollViewer);

                if (mousePos.Y < 30)
                {
                    MainScrollViewer.ScrollToVerticalOffset(
                        MainScrollViewer.VerticalOffset - 20);
                }
                else if (mousePos.Y > MainScrollViewer.ActualHeight - 30)
                {
                    MainScrollViewer.ScrollToVerticalOffset(
                        MainScrollViewer.VerticalOffset + 20);
                }
            };
        }

        private void ToggleTree_Click(object sender, RoutedEventArgs e)
        {
            if (_treeVisible)
            {
                _wasTreeWidth = TreeColumn.Width;
                TreeColumn.Width = new GridLength(0);
            }
            else
            {
                TreeColumn.Width = _wasTreeWidth;
            }

            _treeVisible = !_treeVisible;
        }

        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Прокручиваем всегда, независимо от того, где мышь
            MainScrollViewer.ScrollToVerticalOffset(
                MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void NavigationTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            ScrollToItem(e.NewValue);
        }

        private void ScrollToItem(object item)
        {
            var container = FindVisualChild<FrameworkElement>(MainScrollViewer, item);

            container?.BringIntoView();
        }

        private T FindVisualChild<T>(DependencyObject parent, object dataContext) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T element && element.DataContext == dataContext)
                    return element;

                var result = FindVisualChild<T>(child, dataContext);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
