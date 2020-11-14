using System;

namespace ModelLayer
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal SolutionRating { get; set; }
        public bool Anonymous { get; set; }
    }
}
