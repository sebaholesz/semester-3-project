using DesktopApplication.Communication;
using System.Windows;

namespace DesktopApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            DataGridUsers.ItemsSource = UserApi.GetAllUsers();
        }
    }
}
