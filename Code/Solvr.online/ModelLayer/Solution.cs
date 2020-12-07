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

        public Solution(int assignmentId, string description, DateTime timestamp, bool anonymous, byte[] solutionFile, string userId, decimal solutionRating = 0)
        {
            this.AssignmentId = assignmentId;
            this.Description = description;
            this.Timestamp = timestamp;
            this.Anonymous = anonymous;
            this.SolutionFile = solutionFile;
            this.UserId = userId;
            this.SolutionRating = solutionRating;
        }

        public Solution(int assignmentId, string description, DateTime timestamp, bool anonymous, string userId, decimal solutionRating = 0)
        {
            this.AssignmentId = assignmentId;
            this.Description = description;
            this.Timestamp = timestamp;
            this.Anonymous = anonymous;
            this.UserId = userId;
            this.SolutionRating = solutionRating;
        }

        public Solution()
        {
        }
    }
}
