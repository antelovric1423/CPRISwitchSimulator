using System;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace CPRISwitchSimulator
{
    /// <summary>
    /// Interaction logic for CpriElementConfigurator.xaml
    /// </summary>
    public partial class ElementConfigurator : Window
    {
        public enum Result
        {
            DELETE,
            OK
        }
        public ElementConfigurator(TopologyModel.Element element)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _viewModel = new ElementConfiguratorViewModel(element);
            DataContext = _viewModel;
            ConfiguratorResult = Result.OK;
        }
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CpriElementName.Text) || !char.IsLetter(CpriElementName.Text[0]))
            {
                _ = MessageBox.Show("Invalid element name!");
                return;
            }

            List<TopologyModel.Port> connectorPorts = ElementGetPortsOfType(TopologyModel.PortType.CONNECTOR_PORT);

            if (connectorPorts.Count == 0)
            {
                _ = MessageBox.Show("Element must at least have 1 connector type port!");
                return;
            }

            switch (_viewModel.Element.Type)
            {
                case TopologyModel.ElementType.REC:
                    List<TopologyModel.Port> processingPorts = ElementGetPortsOfType(TopologyModel.PortType.PROCESSING_PORT);

                    if (processingPorts.Count == 0)
                    {
                        _ = MessageBox.Show("REC must at least have 1 processing type port!");
                        return;
                    }
                    break;
                case TopologyModel.ElementType.RE:
                    break;
                case TopologyModel.ElementType.SWITCH:
                    if (connectorPorts.Count < 2)
                    {
                        _ = MessageBox.Show("Switch must at least have 2 connector type ports!");
                        return;
                    }
                    break;
                default:
                    throw new Exception("Invalid element type");
            }

            ConfiguratorResult = Result.OK;
            Close();
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ConfiguratorResult = Result.DELETE;
            Close();
        }
        private void NewPortAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewPortNameTextBox.Text)
                || !char.IsLetter(NewPortNameTextBox.Text[0]))
            {
                _ = MessageBox.Show("Invalid port name");
                return;
            }
            if (NewPortTypeComboBox.SelectedItem == null)
            {
                _ = MessageBox.Show("Invalid port type");
                return;
            }
            if (NewPortCapacityComboBox.SelectedItem == null)
            {
                _ = MessageBox.Show("Invalid port capacity");
                return;
            }


            TopologyModel.PortType type = (TopologyModel.PortType)NewPortTypeComboBox.SelectedItem;
            TopologyModel.LineRate capacity = (TopologyModel.LineRate)NewPortCapacityComboBox.SelectedItem;

            Console.WriteLine("Port type: " + type);
            Console.WriteLine("Port capacity: " + capacity);

            string name = NewPortNameTextBox.Text;

            try
            {
                _viewModel.AddPort(name, type, capacity);
            }
            catch (Exception exception)
            {
                _ = MessageBox.Show(exception.Message);
                return;
            }
        }
        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            if (e.Key == Key.Delete)
            {
                try
                {
                    e.Handled = true;
                    _viewModel.RemovePort(grid.SelectedItem as TopologyModel.Port);
                    grid.SelectedItem = null;
                }
                catch (Exception exception)
                {
                    _ = MessageBox.Show(exception.Message);
                    return;
                }
            }
        }
        private void InspectCapacity_Click(object sender, RoutedEventArgs e)
        {
            TopologyModel.Port port = (sender as Button).DataContext as TopologyModel.Port;
            TopologyModel.Capacity capacity = null;

            if (port is TopologyModel.ProcessingPort processingPort)
            {
                capacity = processingPort.Capacity;
            }
            else
            {
                if ((port as TopologyModel.ConnectorPort).Link == null)
                {
                    _ = MessageBox.Show("Port is not connected by link, so there is no capacity.");
                    return;
                }

                capacity = (port as TopologyModel.ConnectorPort).Link.Capacity;
            }

            CapacityInspector capacityInspector = new CapacityInspector(capacity);
            _ = capacityInspector.ShowDialog();
        }
        private List<TopologyModel.Port> ElementGetPortsOfType(TopologyModel.PortType portType)
        {
            return _viewModel.Ports.Where(x => x.Type == portType).ToList();
        }

        private readonly ElementConfiguratorViewModel _viewModel;
        public Result ConfiguratorResult { get; private set; }
    }
}
