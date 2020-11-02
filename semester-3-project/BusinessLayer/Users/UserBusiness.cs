using DataAccessLayer;
using DataAccessLayer.RepositoryLayer;
using ModelLayer.User;
using System.Collections.Generic;

namespace BusinessLayer.Users
{
    public class UserBusiness
    {
        DbUserIF dbUser = new DbUser();
        public List<User> GetAllUsers()
        {
            return dbUser.GetAllUsers();
        }

        public int InsertUser(User user)
        {
            return dbUser.InsertUser(user);
        }
    }
}
