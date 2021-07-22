using System.ComponentModel;
using System.Windows;

namespace CPRISwitchSimulator
{
    /// <summary>
    /// Interaction logic for CloudConfigurator.xaml
    /// </summary>
    public partial class CloudConfigurator : Window
    {
        public enum Result
        {
            DELETE,
            OK
        }
        public CloudConfigurator(CloudViewModel cloudVM)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = cloudVM;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CloudName.Text) && !char.IsLetter(CloudName.Text[0]))
            {
                _ = MessageBox.Show("Invalid name!");
                return;
            }

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
