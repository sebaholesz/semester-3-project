using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbUser
    {
        User GetDisplayDataByUserId(string userId);
        string GetUserUsername(string userId);
        int GetUserCredits(string userId);
        int UpdateUserCredits(int credits, string userId);
        string GetUserName(string userId);
        bool CheckIfUserExists(string userId);
        List<User> GetAllUsers();
        string GetUserHashedPasswordByUserName(string userUserName);
        string GetUserRoleByUserName(string userUserName);
        User GetUserByUserName(string userUserName);
        bool AuthenticateUserWithIdAndSecurityStamp(User userToAuthenticate);
        User GetUserById(string userId);
        //int InsertUser(User user);
        //int UpdateUser(User user, int id);
        //User GetUserById(int id);
        //int DeleteUser(int id);
    }
}
