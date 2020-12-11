using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ASP.NET.Controllers
{
    public static class AuthenticationController
    {
        public static string CreateJWT(string username, string password)
        {

            using (HttpClient client = new HttpClient())
            {
                string urlCreateJWT = "https://localhost:44316/apiV1/login";
                HttpResponseMessage createJWTRM = client.PostAsync(urlCreateJWT, new StringContent(JsonConvert.SerializeObject(new { username, password }), Encoding.UTF8, "application/json")).Result;

                if (createJWTRM.IsSuccessStatusCode)
                {
                    dynamic tokenObject = createJWTRM.Content.ReadAsAsync<object>().Result;
                    string token = tokenObject.token;

                    if (!token.Equals("") && token.Length > 0)
                    {
                        return token;
                    }
                }
                return "";
            }
        }
    }
}
