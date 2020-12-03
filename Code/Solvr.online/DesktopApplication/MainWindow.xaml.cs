using BusinessLayer;
using DesktopApplication.Communication;
using ModelLayer;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
    }
}
