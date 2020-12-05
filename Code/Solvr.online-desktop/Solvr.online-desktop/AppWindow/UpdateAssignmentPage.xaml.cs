using Solvr.online_desktop.ApiCalls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for UpdateAssignmentPage.xaml
    /// </summary>
    public partial class UpdateAssignmentPage : Page
    {
        private readonly MainWindow mw;
        private readonly int id;
        public UpdateAssignmentPage(int assignmentId, string title, string description, int price, DateTime postDate, DateTime deadline, Boolean anonymous, string academicLevel, string subject)
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
            id = assignmentId;
            TextBoxTitle.Text = title;
            TextBoxDescription.Text = description;
            //TextBoxAuthor.Text = author;
            TextBoxPrice.Text = price.ToString();
            TextBoxDeadline.Text = deadline.ToString();
            TextBoxPostDate.Text = postDate.ToString();
            CheckBoxAnonymous.IsChecked = anonymous;
            TextBoxAcademicLevel.Text = academicLevel;
            TextBoxSubject.Text = subject;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = new AssignmentWindow();
        }

        private void ButtonUpdateAssignment_Click(object sender, RoutedEventArgs e)
        {
            ApiAssignment.UpdateAssignment(id, TextBoxTitle.Text, TextBoxDescription.Text, Convert.ToInt32(TextBoxPrice.Text),
                DateTime.Parse(TextBoxDeadline.Text), (bool)CheckBoxAnonymous.IsChecked, TextBoxAcademicLevel.Text, TextBoxSubject.Text);
        }
    }
}
