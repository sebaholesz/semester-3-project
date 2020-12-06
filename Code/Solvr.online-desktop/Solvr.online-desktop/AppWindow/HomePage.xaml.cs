using Solvr.online_desktop.ApiCalls;
using Solvr.online_desktop.Models;
using System;
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
        public HomePage()
        {
            InitializeComponent();
            _apiAssignment = new ApiAssignment();
            mw = (MainWindow)Application.Current.MainWindow;
        }

        private void ButtonAllAssignments_Click(object sender, RoutedEventArgs e)
        {
            DataGridAssignments.ItemsSource = _apiAssignment.GetAllAssignments();
            DataGridAssignments.Visibility = Visibility.Visible;
            ButtonUpdate.Visibility = Visibility.Visible;
            ButtonMakeActive.Visibility = Visibility.Visible;
            ButtonMakeInactive.Visibility = Visibility.Visible;
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
                string academicLevel = assignment.AcademicLevel;
                string subject = assignment.Subject;
                mw.FrameDefault.Content = new UpdateAssignmentPage(assignmentId, title, description, price, postDate, deadline, anonymous, academicLevel, subject);
            }
        }

        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
        }
    }
}
