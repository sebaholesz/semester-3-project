using DesktopApplication.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DesktopApplication.Communication
{
    class UserApi
    {
        public static IEnumerable<User> GetAllUsers()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("https://localhost:44383/api/user").Result;
            if (response.IsSuccessStatusCode)
            {
                client.Dispose();
                return response.Content.ReadAsAsync<IEnumerable<User>>().Result;
            }
            else
            {
                client.Dispose();
                return null;
            }
        }
    }
}
