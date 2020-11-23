using Newtonsoft.Json;
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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage message = client.GetAsync("https://localhost:44383/api/academiclevel").Result;
                    string responseContent = message.Content.ReadAsStringAsync().Result;
                    List<string> academicLevels = JsonConvert.DeserializeObject<List<string>>(responseContent);

                    if (academicLevels is List<string>)
                    {
                        return academicLevels;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<string> Subjects()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage messageSubject = client.GetAsync("https://localhost:44383/api/subject").Result;
                    string responseContentSubject = messageSubject.Content.ReadAsStringAsync().Result;
                    List<string> subjects = JsonConvert.DeserializeObject<List<string>>(responseContentSubject);

                    if (subjects is List<string>)
                    {
                        return subjects;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}