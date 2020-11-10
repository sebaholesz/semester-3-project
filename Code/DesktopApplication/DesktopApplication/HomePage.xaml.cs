using DesktopApplication.Communication;
using DesktopApplication.Model;
using System.Windows;
using System.Windows.Controls;

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

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsersAll.SelectedItem is User user)
            {
                int userId = user.UserId;
                string username = user.Username;
                string lastLogin = user.LastLogin;
                string password = user.Password;
                string firstName = user.FirstName;
                string lastName = user.LastName;
                string email = user.Email;
                mw.FrameUser.Content = new UpdateUserPage(userId, username, lastLogin, password, firstName, lastName, email);                
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsersAll.SelectedItem is User user)
            {
                int userId = user.UserId;
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    UserApi.DeleteUser(userId);
                    Read_Click(sender, e);
                }
                else
                {
                    DataGridUsersAll.SelectedItem = null;
                }
            }
        }
    }
}