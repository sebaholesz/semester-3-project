using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class Assignment
    {       
        public int AssignmentId { get; set; }
        [Required]
        [StringLength(75)]
        public string Title { get; set; }
        [Required]
        [StringLength(1200)]
        public string Description { get; set; }
        public string UserId { get; set; }
        [Required]
        [Range(0, 10000)]
        public int Price { get; set; }
        public DateTime PostDate { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        public bool Anonymous { get; set; }
        [Required]
        public string AcademicLevel { get; set; }
        [Required]
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public byte[] AssignmentFile { get; set; }
        
        public byte[] Timestamp { get; set; }

        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject, byte[] assignmentFile, string userId)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.AssignmentFile = assignmentFile;
            this.UserId = userId;
        }

        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject, string userId)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.UserId = userId;
        }
        
        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject, byte[] assignmentFile, string userId, bool isActive)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.AssignmentFile = assignmentFile;
            this.UserId = userId;
            this.IsActive = isActive;
        }
        
        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject, string userId, bool isActive)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.UserId = userId;
            this.IsActive = isActive;
        }
        
        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject, byte[] assignmentFile)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
            this.AssignmentFile = assignmentFile;
        }
        
        public Assignment(string title, string description, int price, DateTime postDate, DateTime deadline, bool anonymous, string academicLevel, string subject)
        {
            this.Title = title;
            this.Description = description;
            this.Price = price;
            this.PostDate = postDate;
            this.Deadline = deadline;
            this.Anonymous = anonymous;
            this.AcademicLevel = academicLevel;
            this.Subject = subject;
        }
        
        public Assignment()
        {
        }
        
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