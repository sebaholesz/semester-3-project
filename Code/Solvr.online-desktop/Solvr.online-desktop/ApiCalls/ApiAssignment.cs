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
        public static IEnumerable<Assignment> GetAllAssignments()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("https://localhost:44316/apiV1/assignment").Result;
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

        public static HttpResponseMessage UpdateAssignment(int assignmentId, string title, string description, int price, DateTime deadline, Boolean anonymous, string academicLevel, string subject)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44316/")
            };
            Assignment assignment = new Assignment(title, description, price, deadline, anonymous, academicLevel, subject);

            string url = "apiV1/assignment/" + assignmentId;
            HttpResponseMessage response = client.PutAsJsonAsync(url, assignment).Result;
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

        public HttpResponseMessage MakeAssignmentActive(int assignmentId, string username)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44316/"),
                
                
            };
            User user = new User
            {
                UserName = username
            };
            var url = "apiV1/assignment/active/" + assignmentId;
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

        public HttpResponseMessage MakeAssignmentInactive(int assignmentId)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44316/")
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

        public static IEnumerable<string> GetAllAcademicLevels()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("https://localhost:44316/apiV1/academiclevel").Result;
            if (response.IsSuccessStatusCode)
            {
                client.Dispose();
                return response.Content.ReadAsAsync<IEnumerable<string>>().Result;
            }
            else
            {
                client.Dispose();
                return null;
            }
        }

        public static IEnumerable<string> GetAllSubjects()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("https://localhost:44316/apiV1/subject").Result;
            if (response.IsSuccessStatusCode)
            {
                client.Dispose();
                return response.Content.ReadAsAsync<IEnumerable<string>>().Result;
            }
            else
            {
                client.Dispose();
                return null;
            }
        }
    }
}
