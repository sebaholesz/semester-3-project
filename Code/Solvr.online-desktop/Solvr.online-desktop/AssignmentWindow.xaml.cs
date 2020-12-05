using Solvr.online_desktop.ApiCalls;
using Solvr.online_desktop.Models;
using System.Windows;
using System.Windows.Controls;

namespace Solvr.online_desktop
{
    /// <summary>
    /// Interaction logic for AssignmentWindow.xaml
    /// </summary>
    public partial class AssignmentWindow : Page
    {
        private readonly ApiAssignment _apiAssignment;
        public AssignmentWindow()
        {
            InitializeComponent();
            _apiAssignment = new ApiAssignment();
        }

        private void ButtonAllAssignments_Click(object sender, RoutedEventArgs e)
        {
            DataGridAssignments.ItemsSource = _apiAssignment.GetAllAssignments();
            DataGridAssignments.Visibility = Visibility.Visible;
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
    }
}
