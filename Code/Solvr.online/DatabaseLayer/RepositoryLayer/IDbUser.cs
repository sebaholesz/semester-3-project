using ModelLayer;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbUser
    {
        User GetDisplayDataByUserId(string userId);
        string GetUserUsername(string userId);
        string GetUserName(string userId);
        bool CheckIfUserExists(string userId);
        //List<User> GetAllUsers();
        //int InsertUser(User user);
        //int UpdateUser(User user, int id);
        //User GetUserById(int id);
        //int DeleteUser(int id);
    }
}
