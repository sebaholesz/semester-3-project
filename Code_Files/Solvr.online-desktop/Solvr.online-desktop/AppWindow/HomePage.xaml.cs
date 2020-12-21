using Solvr.online_desktop.ApiCalls;
using Solvr.online_desktop.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for AssignmentWindow.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly MainWindow _mw;
        private readonly ApiAssignment _apiAssignment;
        private readonly LoginPage _loginPage;

        public HomePage()
        {
            InitializeComponent();
            _apiAssignment = new ApiAssignment();
            _mw = (MainWindow)Application.Current.MainWindow;
            _loginPage = new LoginPage();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ButtonAllAssignments_Click(object sender, RoutedEventArgs e)
        {           
            if (ApiAssignment.GetAllAssignments() != null)
            {
                DataGridForAll.ItemsSource = ApiAssignment.GetAllAssignments();
                DataGridForAll.Columns[2].Width = 300;
                DataGridForAll.Visibility = Visibility.Visible;
                ButtonUpdate.Visibility = Visibility.Visible;
                ButtonRemoveCredits.Visibility = Visibility.Hidden;
                ButtonAddCredits.Visibility = Visibility.Hidden;
                ButtonMakeActive.Visibility = Visibility.Visible;
                ButtonMakeInactive.Visibility = Visibility.Visible;
                DataGridForAll.Columns[10].Visibility = Visibility.Hidden;
            }
            else
            {
                TextBlockNoMessage.Text = "There are no assignments yet!";
                TextBlockNoMessage.Visibility = Visibility.Visible;
            }
        }

        private void ButtonMakeInactive_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForAll.SelectedItem is Assignment assignment)
            {
                int assignmentId = assignment.AssignmentId;
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Make inactive", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _apiAssignment.MakeAssignmentInactive(assignmentId);
                    ButtonAllAssignments_Click(sender, e);
                }
                else
                {
                    DataGridForAll.SelectedItem = null;
                }
            }
        }

        private void ButtonMakeActive_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForAll.SelectedItem is Assignment assignment)
            {
                int assignmentId = assignment.AssignmentId;
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Make active", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _apiAssignment.MakeAssignmentActive(assignmentId);
                    ButtonAllAssignments_Click(sender, e);
                }
                else
                {
                    DataGridForAll.SelectedItem = null;
                }
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForAll.SelectedItem is Assignment assignment)
            {
                int assignmentId = assignment.AssignmentId;
                string title = assignment.Title;
                string description = assignment.Description;
                //string author = assignment.Author;
                int price = assignment.Price;
                DateTime postDate = assignment.PostDate;
                DateTime deadline = assignment.Deadline;
                Boolean anonymous = assignment.Anonymous;
                IEnumerable<string> academicLevel = ApiAssignment.GetAllAcademicLevels();
                IEnumerable<string> subject = ApiAssignment.GetAllSubjects();
                byte[] timestamp = assignment.Timestamp;
                _mw.FrameDefault.Content = new UpdateAssignmentPage(assignmentId, title, description, price, postDate, deadline, anonymous, academicLevel, subject, timestamp);
            }
        }

        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
        }

        private void ButtonSignOut_Click(object sender, RoutedEventArgs e)
        {
            _mw.FrameDefault.Content = _loginPage;
            _mw.Width = 550;
            _mw.Height = 300;
            _loginPage.TextBlockMessage.Text = "You have been successfully logged out!";
            _loginPage.TextBlockMessage.Visibility = Visibility.Visible;
            ApiAuthentication.Logintoken = "";
        }

        internal void ButtonAllUsers_Click(object sender, RoutedEventArgs e)
        {
            if (ApiAssignment.GetAllAssignments() != null)
            {
                DataGridForAll.ItemsSource = ApiUser.GetAllUsers();
                DataGridForAll.Columns[0].Visibility = Visibility.Hidden;
                DataGridForAll.Visibility = Visibility.Visible;
                ButtonUpdate.Visibility = Visibility.Visible;
                ButtonMakeActive.Visibility = Visibility.Hidden;
                ButtonMakeInactive.Visibility = Visibility.Hidden;
                ButtonRemoveCredits.Visibility = Visibility.Visible;
                ButtonAddCredits.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockNoMessage.Text = "There are no users yet!";
                TextBlockNoMessage.Visibility = Visibility.Visible;
            }

        }

        private void ButtonRemoveCredits_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForAll.SelectedItem is User user)
            {
                CreditsWindow cw = new CreditsWindow(user.Id, user.Credit)
                {
                    Title = "Remove Credits"
                };
                cw.Show();
                cw.ButtonRemove.Visibility = Visibility.Visible;
            }
        }

        private void ButtonAddCredits_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridForAll.SelectedItem is User user)
            {
                CreditsWindow cw = new CreditsWindow(user.Id, user.Credit)
                {
                    Title = "Add Credits"
                };
                cw.Show();
                cw.ButtonAdd.Visibility = Visibility.Visible;                
            }
        }
    }
}
