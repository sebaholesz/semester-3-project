namespace DesktopApplication.Model
{
    class User
    {
        public User(string username, string lastLogin, string password, string firstName, string lastName, string email)
        {
            Username = username;
            LastLogin = lastLogin;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string LastLogin { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
