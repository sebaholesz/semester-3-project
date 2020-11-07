using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
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

        public int UpdateUser(User user, int id)
        {
            return dbUser.UpdateUser(user, id);
        }

        public User GetUserById(int id)
        {
            return dbUser.GetUserById(id);
        }

        public int DeleteUser(int id)
        {
            return dbUser.DeleteUser(id);
        }
    }
}
