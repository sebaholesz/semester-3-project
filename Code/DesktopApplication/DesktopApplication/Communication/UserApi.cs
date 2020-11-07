using System.Net.Http;
using System.Net.Http.Headers;

namespace DesktopApplication.Communication
{
    class UserApi
    {
        public static string GetAllUsers()
        {
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("https://localhost:44383/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("http://localhost:44383/api/user").Result;
            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsStringAsync().Result;
                // response.Content.ReadAsAsync<IEnumerable<BabisUser>>().Result;
                client.Dispose();
                return users;
            }
            else
            {
                client.Dispose();
                return "";
            }
        }
    }
}
