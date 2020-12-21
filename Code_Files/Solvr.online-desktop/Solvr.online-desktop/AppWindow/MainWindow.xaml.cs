using System.Windows;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameDefault.Content = new LoginPage();
        }
    }
}
