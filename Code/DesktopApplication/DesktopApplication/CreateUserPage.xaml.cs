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
    /// Interaction logic for CreateUserPage.xaml
    /// </summary>
    public partial class CreateUserPage : Page
    { 
        MainWindow mw;
        public CreateUserPage()
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
        }

        private void Button_Go_Back(object sender, RoutedEventArgs e)
        {
            mw.FrameUser.Content = new HomePage();
        }

        private void Button_Create_User(object sender, RoutedEventArgs e)
        {
            UserApi.CreateUser(Username.Text, LastLogin.Text, Password.Text, FirstName.Text, LastLogin.Text, Email.Text);
        }
    }
}
    