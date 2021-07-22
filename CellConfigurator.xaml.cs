using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System;

namespace CPRISwitchSimulator
{
    /// <summary>
    /// Interaction logic for CloudConfigurator.xaml
    /// </summary>
    public partial class CellConfigurator : Window
    {
        public enum Result
        {
            DELETE,
            OK
        }
        public CellConfigurator(CellMainViewModel cellMainVM)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _configuratorVM = new CellConfiguratorViewModel(cellMainVM.Cell);
            DataContext = _configuratorVM;
            BandwidthCBox.SelectionChanged += BandwidthCBox_OnSelectionChanged;
            RatTypeCBox.SelectionChanged += RatTypeCBox_OnSelectionChanged;
            ConfiguratorResult = Result.OK;
        }

        private void BandwidthCBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TopologyModel.CarrierBandwidth newBW;
            if (BandwidthCBox.SelectedItem == null)
                newBW = TopologyModel.CarrierBandwidth.NOT_SET;
            else
                newBW = (TopologyModel.CarrierBandwidth)BandwidthCBox.SelectedItem;
            try
            {
                _configuratorVM.Bandwidth = newBW;
            }

            catch (Exception exception)
            {
                BandwidthCBox.SelectedItem = _configuratorVM.Bandwidth;
                _ = MessageBox.Show(exception.Message);
            }
        }
        private void RatTypeCBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TopologyModel.RatType newRatType;
            if (RatTypeCBox.SelectedItem == null)
                newRatType = TopologyModel.RatType.NOT_SET;
            else
                newRatType = (TopologyModel.RatType)RatTypeCBox.SelectedItem;

            try
            {
                _configuratorVM.RatType = newRatType;
            }
            catch (Exception exception)
            {
                RatTypeCBox.SelectedItem = _configuratorVM.RatType;
                _ = MessageBox.Show(exception.Message);
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (ConfiguratorResult == Result.OK)
            {
                if (string.IsNullOrEmpty(CellName.Text) || !char.IsLetter(CellName.Text[0]))
                {
                    _ = MessageBox.Show("Invalid name!");
                    e.Cancel = true;
                    return;
                }

                if (_configuratorVM.RatType == TopologyModel.RatType.NOT_SET)
                {
                    _ = MessageBox.Show("RAT type must be set!");
                    e.Cancel = true;
                    return;
                }

                if (_configuratorVM.Bandwidth == TopologyModel.CarrierBandwidth.NOT_SET)
                {
                    _ = MessageBox.Show("Bandwidth must be set!");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ConfiguratorResult = Result.DELETE;
            Close();
        }

        private CellConfiguratorViewModel _configuratorVM;
        public Result ConfiguratorResult { get; private set; }
    }
}
