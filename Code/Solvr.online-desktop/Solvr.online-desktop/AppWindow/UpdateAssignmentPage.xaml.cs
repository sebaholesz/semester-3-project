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
        private readonly string username;

        public UpdateAssignmentPage(int assignmentId, string title, string description, int price, DateTime postDate, DateTime deadline, Boolean anonymous, IEnumerable<string> academicLevel, IEnumerable<string> subject, string username)
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
            id = assignmentId;
            TextBoxTitle.Text = title;
            TextBoxDescription.Text = description;
            //TextBoxAuthor.Text = author;
            TextBoxPrice.Text = price.ToString();
            TextBoxPostDate.Text = postDate.ToString();
            DatePickerDeadline.Text = deadline.ToString();
            CheckBoxAnonymous.IsChecked = anonymous;
            ComboBoxAcademicLevel.ItemsSource = academicLevel;
            ComboBoxSubject.ItemsSource = subject;
            this.username = username;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            mw.FrameDefault.Content = new HomePage(username);
        }

        private void ButtonUpdateAssignment_Click(object sender, RoutedEventArgs e)
        {
            ApiAssignment.UpdateAssignment(id, TextBoxTitle.Text, TextBoxDescription.Text, Convert.ToInt32(TextBoxPrice.Text),
                DatePickerDeadline.DisplayDate, (bool)CheckBoxAnonymous.IsChecked, ComboBoxAcademicLevel.Text, ComboBoxSubject.Text);
        }
    }
}
