using System;
using System.Collections.Generic;
using System.Text;

namespace Solvr.online_desktop.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime Deadline { get; set; }
        public Boolean Anonymous { get; set; }
        public string AcademicLevel { get; set; }
        public string Subject { get; set; }
        public byte[] AssignmentFile { get; set; }
        //public List<Solution> Solutions { get; set; }
    }
}
