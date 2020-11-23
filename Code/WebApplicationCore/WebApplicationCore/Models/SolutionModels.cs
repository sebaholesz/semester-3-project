using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationCore.Models
{
    public class SolutionModels : Controller
    {
        public int SolutionId { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal SolutionRating { get; set; }
        public bool Anonymous { get; set; }
    }
}
