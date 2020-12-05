using Solvr.online_desktop.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Solvr.online_desktop.ApiCalls
{
    public class ApiAssignment
    {
        public IEnumerable<Assignment> GetAllAssignments()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("https://localhost:44395/apiV1/assignment").Result;
            if (response.IsSuccessStatusCode)
            {
                client.Dispose();
                return response.Content.ReadAsAsync<IEnumerable<Assignment>>().Result;
            }
            else
            {
                client.Dispose();
                return null;
            }
        }

        public static HttpResponseMessage UpdateAssignment(int assignmentId)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44395/")
            };
            //User user = new User(username, lastLogin, password, firstName, lastName, email);

            string url = "apiV1/assignment/" + assignmentId;
            HttpResponseMessage response = client.PutAsJsonAsync(url, assignmentId).Result;
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

        public HttpResponseMessage MakeAssignmentActive(int assignmentId)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44395/")
            };
            var url = "apiV1/assignment/iactive/" + assignmentId;
            HttpResponseMessage response = client.PutAsJsonAsync(url, assignmentId).Result;
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

        public HttpResponseMessage MakeAssignmentInactive(int assignmentId)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44395/")
            };
            var url = "apiV1/assignment/inactive/" + assignmentId;
            HttpResponseMessage response = client.PutAsJsonAsync(url, assignmentId).Result;
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
