using Newtonsoft.Json;
using Solvr.online_desktop.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Solvr.online_desktop.ApiCalls
{
    public class ApiAuthentication
    {
        public static string Logintoken;
        private static readonly string _baseUrl = "https://localhost:44316/apiV1/";

        public static bool Login(string username, string password)
        {
            using HttpClient client = new HttpClient();
            string urlCreateJWT = _baseUrl + "login-admin";
            HttpResponseMessage createJWTRM = client.PostAsync(urlCreateJWT, new StringContent(JsonConvert.SerializeObject(new { username, password }), Encoding.UTF8, "application/json")).Result;

            if (createJWTRM.IsSuccessStatusCode)
            {
                dynamic tokenObject = createJWTRM.Content.ReadAsAsync<object>().Result;
                string token = tokenObject.token;

                if (!token.Equals("") && token.Length > 0)
                {
                    Logintoken = token;
                    return true;
                }
            }
            return false;
        }
    }
}
