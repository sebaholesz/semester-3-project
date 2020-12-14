using System;

namespace Solvr.online_desktop.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string Author { get; set; }
        public int Price { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime Deadline { get; set; }
        public bool Anonymous { get; set; }
        public string AcademicLevel { get; set; }
        public string Subject { get; set; }
        public bool IsActive { get; set; }
        public byte[] Timestamp { get; set; }

        public Assignment(string title, string description, int price, DateTime deadline, bool anonymous, string academicLevel, string subject, byte[] timestamp)
        {
            Title = title;
            Description = description;
            //Author = author;
            Price = price;
            Deadline = deadline;
            Anonymous = anonymous;
            AcademicLevel = academicLevel;
            Subject = subject;
            Timestamp = timestamp;
        }
    }
}