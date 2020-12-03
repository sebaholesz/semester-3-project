using DesktopApplication.Communication;
using ModelLayer;
using System.Windows;

namespace DesktopApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiAssignmnet _apiAssignment;
        //private readonly AssignmentBusiness ab;
        public MainWindow()
        {
            _apiAssignment = new ApiAssignmnet();
            //ab = new AssignmentBusiness();
            InitializeComponent();
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            AssignmentTable.ItemsSource = _apiAssignment.GetAllAssignments();
            //AssignmentTable.ItemsSource = ab.GetAllAssignments();
        }

        private void MakeInactive_Click(object sender, RoutedEventArgs e)
        {
            if (AssignmentTable.SelectedItem is Assignment assignment)
            {
                int assignmentId = assignment.AssignmentId;
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Make inactive", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _apiAssignment.MakeAssignmentInactive(assignmentId);
                    Read_Click(sender, e);
                }
                else
                {
                    AssignmentTable.SelectedItem = null;
                }
            }
        }
    }
}
