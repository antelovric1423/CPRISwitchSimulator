using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPRISwitchSimulator
{
    /// <summary>
    /// Interaction logic for ConnectionConfigurator.xaml
    /// </summary>
    public partial class ElementConnectionConfigurator : Window
    {
        public enum Result
        {
            DELETE,
            OK
        }
        public ElementConnectionConfigurator(ElementConnectionMainViewModel connectionMainViewModel)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _viewModel = new ElementConnectionConfiguratorViewModel(connectionMainViewModel);
            DataContext = _viewModel;
            ConfiguratorResult = Result.OK;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_viewModel.Links.Count == 0)
                ConfiguratorResult = Result.DELETE;
        }
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Links.Count == 0)
            {
                _ = MessageBox.Show("Can't create connection with 0 links!");
                return;
            }

            Close();
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.RemoveAllLinks();
            }
            catch (Exception exception)
            {
                _ = MessageBox.Show(exception.Message);
                return;
            }

            Close();
        }
        private void NewLinkAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (Element1Port.SelectedItem == null)
            {
                _ = MessageBox.Show("Invalid first element port");
                return;
            }
            if (Element2Port.SelectedItem == null)
            {
                _ = MessageBox.Show("Invalid second element port");
                return;
            }

            _viewModel.AddLink((TopologyModel.ConnectorPort)Element1Port.SelectedItem, (TopologyModel.ConnectorPort)Element2Port.SelectedItem);
        }
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (e.Key == Key.Delete)
            {
                try
                {
                    e.Handled = true;
                    _viewModel.RemoveLink(grid.SelectedItem as TopologyModel.Link);
                    grid.SelectedItem = null;
                }
                catch (Exception exception)
                {
                    _ = MessageBox.Show(exception.Message);
                    return;
                }
            }
        }
        public Result ConfiguratorResult { get; private set; }
        private ElementConnectionConfiguratorViewModel _viewModel;

        private void InspectCapacity_Click(object sender, RoutedEventArgs e)
        {
            TopologyModel.Link link = (sender as Button).DataContext as TopologyModel.Link;

            CapacityInspector capacityInspector = new CapacityInspector(link.Capacity);
            _ = capacityInspector.ShowDialog();
        }
    }
}
