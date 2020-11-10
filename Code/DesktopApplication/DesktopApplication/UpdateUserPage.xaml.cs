using DesktopApplication.Communication;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApplication
{
    /// <summary>
    /// Interaction logic for UpdateUser.xaml
    /// </summary>
    public partial class UpdateUserPage : Page
    {
        MainWindow mw;
        int id;
        public UpdateUserPage(int userId, string username, string lastLogin, string password, string firstName, string lastName, string email)
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
            id = userId;
            Username.Text = username;
            LastLogin.Text = lastLogin;
            Password.Text = password;
            FirstName.Text = firstName;
            LastName.Text = lastName;
            Email.Text = email;
        }

        private void Button_Update_User(object sender, RoutedEventArgs e)
        {
            UserApi.UpdateUser(id, Username.Text, LastLogin.Text, Password.Text, FirstName.Text, LastLogin.Text, Email.Text);
            MessageBox.Show("User was updated successfully!", "Update", MessageBoxButton.OK);
            mw.FrameUser.Content = new HomePage();
        }

        private void Button_Go_Back(object sender, RoutedEventArgs e)
        {
            mw.FrameUser.Content = new HomePage();
        }
    }
}
