using System;
using System.Collections.Generic;
using System.Text;

namespace Solvr.online_desktop.Models
{
    class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Rating { get; set; }
        public int Credit { get; set; }
        //public string Access_token { get; set; }
    }
}
