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
    /// Interaction logic for CapacityInspector.xaml
    /// </summary>
    public partial class CapacityInspector : Window
    {
        public CapacityInspector(TopologyModel.Capacity capacity)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _viewModel = new CapacityInspectorViewModel(capacity);
            DataContext = _viewModel;
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private readonly CapacityInspectorViewModel _viewModel;
    }
}
