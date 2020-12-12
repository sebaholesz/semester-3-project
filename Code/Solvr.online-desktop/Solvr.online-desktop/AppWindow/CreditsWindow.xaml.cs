using Solvr.online_desktop.ApiCalls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for CreditsWindow.xaml
    /// </summary>
    public partial class CreditsWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private readonly string _userId;

        public CreditsWindow(string userId)
        {
            InitializeComponent();
            _userId = userId;
        }
        
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string value = TextBoxEditCredits.Text;
            if (IsTextAllowed(value))
            {
                int creditValue = Convert.ToInt32(value);
                ApiUser.AddCredits(creditValue, _userId);
                this.Close();
            }
            else
            {
                TextBlockEnterNumber.Visibility = Visibility.Visible;
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            string value = TextBoxEditCredits.Text;
            if (IsTextAllowed(value))
            {
                int creditValue = Convert.ToInt32(value);
                ApiUser.RemoveCredits(creditValue, _userId);
                this.Close();
            }
            else
            {
                TextBlockEnterNumber.Visibility = Visibility.Visible;
            }
        }
    }
}
