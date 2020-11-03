using ModelLayer.User;
using System.Collections.Generic;

namespace DataAccessLayer.RepositoryLayer
{
    public interface DbUserIF
    {
        List<User> GetAllUsers();
        int InsertUser(User user);
        int UpdateUser(User user, int id);
        User GetUserById(int id);
        int DeleteUser(int id);
    }
}
