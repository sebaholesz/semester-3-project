using System.Windows;
using System.Windows.Controls;

namespace Solvr.online_desktop
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly MainWindow mw;
        public HomePage()
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
        }

        private void ButtonAssignment_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = new AssignmentWindow();
        }
    }
}
