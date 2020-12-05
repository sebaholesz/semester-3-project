using System.Windows;

namespace Solvr.online_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameDefault.Content = new HomePage();
        }
    }
}
