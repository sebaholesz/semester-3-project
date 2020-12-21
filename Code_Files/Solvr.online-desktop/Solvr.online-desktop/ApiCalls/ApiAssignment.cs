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
        private static readonly string _baseUrl = "https://localhost:44316/apiV1/";

        public static IEnumerable<Assignment> GetAllAssignments()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.GetAsync(_baseUrl + "assignment").Result;
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

        public static HttpResponseMessage UpdateAssignment(int assignmentId, string title, string description, int price, DateTime deadline, Boolean anonymous, string academicLevel, string subject, byte[] timestamp)
        {
            HttpClient client = new HttpClient();
            Assignment assignment = new Assignment(title, description, price, deadline, anonymous, academicLevel, subject, timestamp);

            string url = _baseUrl + "assignment-admin/" + assignmentId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
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

        public HttpResponseMessage MakeAssignmentActive(int assignmentId)
        {
            HttpClient client = new HttpClient();
            
            var url = _baseUrl + "assignment-admin/active/" + assignmentId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
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
            HttpClient client = new HttpClient();
            var url = _baseUrl + "assignment-admin/inactive/" + assignmentId;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.GetAsync(_baseUrl + "academiclevel").Result;
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthentication.Logintoken);
            HttpResponseMessage response = client.GetAsync(_baseUrl + "subject").Result;
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
