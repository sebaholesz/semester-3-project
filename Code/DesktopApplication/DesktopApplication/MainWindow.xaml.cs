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
            //FrameUser.Navigate( new HomePage());
            FrameUser.Content = new HomePage();
        }
    }
}
