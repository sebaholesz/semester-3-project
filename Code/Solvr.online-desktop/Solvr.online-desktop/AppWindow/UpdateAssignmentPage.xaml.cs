using Solvr.online_desktop.ApiCalls;
using System;
using System.Collections.Generic;
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
        public UpdateAssignmentPage(int assignmentId, string title, string description, int price, DateTime postDate, DateTime deadline, Boolean anonymous, IEnumerable<string> academicLevel, IEnumerable<string> subject)
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
            id = assignmentId;
            TextBoxTitle.Text = title;
            TextBoxDescription.Text = description;
            //TextBoxAuthor.Text = author;
            TextBoxPrice.Text = price.ToString();
            DateTimeDeadline.DateTime = deadline;
            TextBoxPostDate.Text = postDate.ToString();
            CheckBoxAnonymous.IsChecked = anonymous;
            ComboBoxAcademicLevel.ItemsSource = academicLevel;
            ComboBoxSubject.ItemsSource = subject;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = new HomePage();
        }

        private void ButtonUpdateAssignment_Click(object sender, RoutedEventArgs e)
        {
            ApiAssignment.UpdateAssignment(id, TextBoxTitle.Text, TextBoxDescription.Text, Convert.ToInt32(TextBoxPrice.Text),
                (DateTime)DateTimeDeadline.DateTime, (bool)CheckBoxAnonymous.IsChecked, ComboBoxAcademicLevel.Text, ComboBoxSubject.Text);
        }
    }
}
