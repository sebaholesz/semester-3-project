using DesktopApplication.Model;
using System;
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

            User user = new User(username, lastLogin, password, firstName, lastName, email);

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

        public static HttpResponseMessage UpdateUser(int userId, string username, string lastLogin, string password, string firstName, string lastName, string email)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44383/");
            User user = new User(username, lastLogin, password, firstName, lastName, email);

            string url = "api/user/" + userId;
            HttpResponseMessage response = client.PutAsJsonAsync(url, user).Result;
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

        public static HttpResponseMessage DeleteUser(int userId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44383/");
            var url = "api/user/" + userId;
            HttpResponseMessage response = client.DeleteAsync(url).Result;
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
