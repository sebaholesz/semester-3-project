using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationCore.Models
{
    public class SolutionModels : Controller
    {


        public int SolutionId { get; set; }

        public int AssignmentId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal SolutionRating { get; set; }

        public bool Anonymous { get; set; }
    }
}
