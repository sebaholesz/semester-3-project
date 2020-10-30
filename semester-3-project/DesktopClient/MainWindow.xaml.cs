using DesktopClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309/Users");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("/Users").Result;

            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
                string userOutput ="";
                foreach (var user in users)
                {
                    userOutput += "User: " + user.Id + "\nName + age: " + user.Name + "   " + user.Age +"\n\n"; 
                }
                TextBox.Text = userOutput;

            }
            else
            {
                TextBox.Text = "Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase;
            }
        }
    }
}
