using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.User
{
    public class User
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public User(int age, string name)
        {
            Age = age;
            Name = name;
        }
    }
}
