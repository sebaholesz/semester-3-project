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
        public string CreditConcurrencyStamp { get; set; }
    }
}