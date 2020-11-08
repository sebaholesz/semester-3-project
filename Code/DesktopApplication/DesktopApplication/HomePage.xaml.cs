using DesktopApplication.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApplication
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        MainWindow mw;
        public HomePage()
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            DataGridUsersAll.ItemsSource = UserApi.GetAllUsers();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameUser.Content = new CreateUserPage();
        }
    }
}
