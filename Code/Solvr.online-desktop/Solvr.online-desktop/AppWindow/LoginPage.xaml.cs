using System.Windows;
using System.Windows.Controls;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly MainWindow mw;
        public LoginPage()
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = new HomePage();
            mw.Width = 1280;
            mw.Height = 720;
        }
    }
}
