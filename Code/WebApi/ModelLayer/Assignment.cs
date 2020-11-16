using System;

namespace ModelLayer
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        // public List<Solution> Queue { get; set; }
        public DateTime Deadline { get; set; }
        public Boolean Anonymous { get; set; }
        public string AcademicLevel { get; set; }
        public string Subject { get; set; }
    }
}