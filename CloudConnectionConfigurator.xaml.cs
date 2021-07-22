using System.Windows;

namespace CPRISwitchSimulator
{
    /// <summary>
    /// Interaction logic for ConnectionConfigurator.xaml
    /// </summary>
    public partial class CloudConnectionConfigurator : Window
    {
        public enum Result
        {
            DELETE,
            OK
        }
        public CloudConnectionConfigurator(CloudConnectionViewModel connectionVM)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = connectionVM;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            ConfiguratorResult = Result.OK;
            Close();
        }
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ConfiguratorResult = Result.DELETE;
            Close();
        }

        public Result ConfiguratorResult { get; private set; }
    }
}
