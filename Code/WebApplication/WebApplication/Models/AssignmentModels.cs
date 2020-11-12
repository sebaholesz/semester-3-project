using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AssignmentModels
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        // public List<Solution> Queue { get; set; }
        public DateTime Deadline { get; set; }
        public Boolean Anonymous { get; set; }
        public AcademicLevel AcademicLevel { get; set; }
        public Subject Subject { get; set; }
    }

    public enum AcademicLevel
    {
        Primary1,
        Primary2,
        Primary3,
        Primary4,
        Primary5,
        Primary6,
        Primary7,
        Primary8,
        Primary9,
        HighSchool1,
        HighSchool2,
        HighSchool3,
        HighSchool4,
        University
    }

    public enum Subject
    {
        Mathematics,
        English,
        History,
        Geography,
        Spanish,
        Physics,
        Biology,
        Chemistry
    }
}