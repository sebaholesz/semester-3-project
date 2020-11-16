using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApplicationCore.Models
{
    public class AssignmentModels
    {
        public int AssignmentId { get; set; }
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        public string Author { get; set; }
        [Required]
        [Range(0, 10000)]
        public int Price { get; set; }
        // public List<Solution> Queue { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        public Boolean Anonymous { get; set; }
        [Required]
        public string AcademicLevel { get; set; }
        [Required]
        public string Subject { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Deadline <= DateTime.Now)
            {
                yield return new ValidationResult(
                    $"Deadline has to be in future.",
                    new[] { nameof(Deadline) });
            }
        }

        public static List<string> AcademicLevels()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("https://localhost:44383/api/academiclevel").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<List<string>>().Result;
                }
                else
                {
                    return null;
                }
            }
        }

        public static List<string> Subjects()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("https://localhost:44383/api/subject").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<List<string>>().Result;
                }
                else
                {
                    return null;
                }
            }
        }

        //public static List<string> AcademicLevelValues = new List<string>() {
        //    "Primary School First Grade",
        //    "Primary School Second Grade",
        //    "Primary School Third Grade",
        //    "Primary School Fourth Grade",
        //    "Primary School Fifth Grade",
        //    "Primary School Sixth Grade",
        //    "Primary School Seventh Grade",
        //    "Primary School Eighth Grade",
        //    "Primary School Ninth Grade",
        //    "High School First Grade",
        //    "High School Second Grade",
        //    "High School Third Grade",
        //    "High School Fourth Grade",
        //    "UNIVERSITY BABYYYYY"
        //};

        //public static List<string> SubjectValues = new List<string>() {
        //    "Mathematics",
        //    "English",
        //    "History",
        //    "Geography",
        //    "Spanish",
        //    "Physics",
        //    "Biology",
        //    "Chemistry"
        //};
    }
}