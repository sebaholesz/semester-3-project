using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class Solution
    {
        public int SolutionId { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal SolutionRating { get; set; }
        public bool Anonymous { get; set; }
        public byte[] SolutionFile { get; set; }
        
        public Solution(int assignmentId, int userId, string description, DateTime timestamp, bool anonymous, byte[] solutionFile,decimal solutionRating = 0)
        {
            this.AssignmentId = assignmentId;
            this.UserId = userId;
            this.Description = description;
            this.Timestamp = timestamp;
            this.Anonymous = anonymous;
            this.SolutionRating = solutionRating;
            this.SolutionFile = solutionFile;
        }
        
        public Solution(int assignmentId, int userId, string description, DateTime timestamp, bool anonymous, decimal solutionRating = 0)
        {
            this.AssignmentId = assignmentId;
            this.UserId = userId;
            this.Description = description;
            this.Timestamp = timestamp;
            this.Anonymous = anonymous;
            this.SolutionRating = solutionRating;
        }
        
        public Solution()
        {
        }
    }
}
