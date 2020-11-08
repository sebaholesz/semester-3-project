using DesktopApplication.Model;
using System.Collections.Generic;
using System.Net;
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
        public static HttpResponseMessage CreateUser(string username, string lastLogin, string password, string firstName, string lastName, string email)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            User user = new User( username,  lastLogin,  password,  firstName,  lastName,  email);

            HttpResponseMessage response = client.PostAsJsonAsync("https://localhost:44383/api/user", user).Result;
            if (response.IsSuccessStatusCode)
            {
                client.Dispose();
                return response;
            }
            else
            {
                client.Dispose();
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
        }

    }
}
