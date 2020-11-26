using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ModelLayer
{
    public class Assignment
    {
        public Assignment(string title, string description, int price, DateTime deadline, bool anonymous, string academicLevel, string subject, byte[] assignmentFile)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.AssignmentFile = assignmentFile;
        }

        public Assignment()
        {
        }

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
        public byte[] AssignmentFile ;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Deadline <= DateTime.Now)
            {
                yield return new ValidationResult(
                    $"Deadline has to be in future.",
                    new[] { nameof(Deadline) });
            }
        }
    }
}