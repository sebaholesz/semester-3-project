namespace ModelLayer.User
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string LastLogin { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        //public User(string Username, string LastLogin, string Password, string FirstName, string LastName, string Email)
        //{
        //    Age = age;
        //    Name = name;
        //}
    }
}
