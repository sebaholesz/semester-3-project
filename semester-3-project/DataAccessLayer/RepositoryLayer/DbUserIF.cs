using ModelLayer.User;
using System.Collections.Generic;

namespace DataAccessLayer.RepositoryLayer
{
    public interface DbUserIF
    {
        List<User> GetAllUsers();
        int InsertUser(User user);
    }
}
