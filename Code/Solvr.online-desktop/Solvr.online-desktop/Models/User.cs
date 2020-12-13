using System;
using System.Collections.Generic;
using System.Text;

namespace Solvr.online_desktop.Models
{
    class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string LastLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public int Credit { get; set; }
    }
}
