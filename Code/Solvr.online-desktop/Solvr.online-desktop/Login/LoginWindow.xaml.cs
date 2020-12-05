using System.Windows;
using System.Windows.Controls;

namespace Solvr.online_desktop.LoginWindow
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            AppWindow.MainWindow mw = new AppWindow.MainWindow();
            object content = mw.Content;

            mw.Content = null;
        }
    }
}
