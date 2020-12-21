using Solvr.online_desktop.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Solvr.online_desktop.ApiCalls
{
    class ApiUser
    {
        private static readonly string _baseUrl = "https://localhost:44316/apiV1/";

        public static IEnumerable<User> GetAllUsers()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.GetAsync(_baseUrl + "user").Result;
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

        public static HttpResponseMessage AddCredits(int value, string userId)
        {
            HttpClient client = new HttpClient();
            var url = _baseUrl + "user-admin/add-credit/" + userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.PutAsJsonAsync(url, value).Result;
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

        public static HttpResponseMessage RemoveCredits(int value, string userId)
        {
            HttpClient client = new HttpClient();
            var url = _baseUrl + "user-admin/remove-credit/" + userId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.PutAsJsonAsync(url, value).Result;
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
