using Microsoft.AspNetCore.Identity;

namespace ModelLayer
{
    public class User : IdentityUser
    {
        public string LastLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}