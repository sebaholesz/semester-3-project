using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int AssignmentId { get; set; }
        [Required]
        [MaxLength(1200)]
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal SolutionRating { get; set; }
        public bool Anonymous { get; set; }
        public byte[] SolutionFile { get; set; }
        public string UserId { get; set; }
        public bool Accepted { get; set; }
    }
}
