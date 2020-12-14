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
        private readonly int _credit;

        public CreditsWindow(string userId, int credit)
        {
            InitializeComponent();
            _userId = userId;
            _credit = credit;
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
                TextBlockEnterNumber.Text = "Enter number!";
                TextBlockEnterNumber.Visibility = Visibility.Visible;
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            string value = TextBoxEditCredits.Text;
            if (IsTextAllowed(value))
            {
                int creditValue = Convert.ToInt32(value);
                if (_credit - creditValue >= 0)
                {
                    ApiUser.RemoveCredits(creditValue, _userId);                    
                    this.Close();
                }
                else
                {
                    TextBlockEnterNumber.Text = "Credits cannot go below zero!";
                    TextBlockEnterNumber.Visibility = Visibility.Visible;
                }
            }
            else
            {
                TextBlockEnterNumber.Text = "Enter number!";
                TextBlockEnterNumber.Visibility = Visibility.Visible;
            }
        }
    }
}
