﻿using Solvr.online_desktop.ApiCalls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Solvr.online_desktop.AppWindow
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly MainWindow _mw;
        public LoginPage()
        {
            InitializeComponent();
            _mw = (MainWindow)Application.Current.MainWindow;
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (TextBoxUsername.Text.Length == 0)
            {
                TextBlockMessage.Text = "Enter username!";
                TextBlockMessage.FontSize = 15;
                TextBlockMessage.Visibility = Visibility.Visible;
                TextBoxUsername.Focus();
            }
            else
            {
                if (ApiAuthentication.Login(TextBoxUsername.Text, PasswordBoxPassword.Password))
                {                    
                    _mw.FrameDefault.Content = new HomePage();
                    _mw.Width = 1280;
                    _mw.Height = 720;
                }
                else
                {
                    TextBlockMessage.Text = "Enter valid username";
                    TextBlockMessage.FontSize = 15;
                    TextBlockMessage.Visibility = Visibility.Visible;
                    TextBoxUsername.Focus();
                    Mouse.OverrideCursor = Cursors.Arrow;
                }

            }     
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxUsername.Clear();
            PasswordBoxPassword.Clear();
            TextBlockMessage.Visibility = Visibility.Hidden;
        }
    }
}