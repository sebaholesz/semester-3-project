using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public AcademicLevel AcademicLevel { get; set; }
        public Subject Subject { get; set; }


        public Dictionary<AcademicLevel, string> AcademicLevels = new Dictionary<AcademicLevel, string>()
        {
            { AcademicLevel.Primary1, "Primary School First Grade"},
            { AcademicLevel.Primary2, "Primary School Second Grade"},
            { AcademicLevel.Primary3, "Primary School Third Grade"},
            { AcademicLevel.Primary4, "Primary School Fourth Grade"},
            { AcademicLevel.Primary5, "Primary School Fifth Grade"},
            { AcademicLevel.Primary6, "Primary School Sixth Grade"},
            { AcademicLevel.Primary7, "Primary School Seventh Grade"},
            { AcademicLevel.Primary8, "Primary School Eighth Grade"},
            { AcademicLevel.Primary9, "Primary School Ninth Grade"},
            { AcademicLevel.HighSchool1, "High School First Grade"},
            { AcademicLevel.HighSchool2, "High School Second Grade"},
            { AcademicLevel.HighSchool3, "High School Third Grade"},
            { AcademicLevel.HighSchool4, "High School Fourth Grade"},
            { AcademicLevel.University, "UNIVERSITY BABYYYYY"},
        }; 
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
