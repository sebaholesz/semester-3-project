using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AssignmentModels
    {
        public int AssignmentId { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        // public List<Solution> Queue { get; set; }
        public DateTime Deadline { get; set; }
        public Boolean Anonymous { get; set; }
    }
}