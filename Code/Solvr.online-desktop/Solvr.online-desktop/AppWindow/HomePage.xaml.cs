using Solvr.online_desktop.ApiCalls;
using Solvr.online_desktop.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for AssignmentWindow.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly MainWindow mw;
        private readonly ApiAssignment _apiAssignment;
        private readonly LoginPage loginPage;
        public HomePage()
        {
            InitializeComponent();
            _apiAssignment = new ApiAssignment();
            mw = (MainWindow)Application.Current.MainWindow;
            loginPage = new LoginPage();
        }

        private void ButtonAllAssignments_Click(object sender, RoutedEventArgs e)
        {
            if (ApiAssignment.GetAllAssignments() != null)
            {
                DataGridAssignments.ItemsSource = ApiAssignment.GetAllAssignments();
                DataGridAssignments.Columns[2].Width = 300;
                DataGridAssignments.Visibility = Visibility.Visible;
                ButtonUpdate.Visibility = Visibility.Visible;
                ButtonMakeActive.Visibility = Visibility.Visible;
                ButtonMakeInactive.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlockNoAssignments.Visibility = Visibility.Visible;
            }
        }

        private void ButtonMakeInactive_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAssignments.SelectedItem is Assignment assignment)
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
                    DataGridAssignments.SelectedItem = null;
                }
            }
        }

        private void ButtonMakeActive_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAssignments.SelectedItem is Assignment assignment)
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
                    DataGridAssignments.SelectedItem = null;
                }
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAssignments.SelectedItem is Assignment assignment)
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
                mw.FrameDefault.Content = new UpdateAssignmentPage(assignmentId, title, description, price, postDate, deadline, anonymous, academicLevel, subject);
            }
        }

        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
        }

        private void ButtonSignOut_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = loginPage;
            mw.Width = 550;
            mw.Height = 300;
            loginPage.TextBlockMessage.Text = "You have been successfully logged out!";
            loginPage.TextBlockMessage.Visibility = Visibility.Visible;
        }
    }
}
