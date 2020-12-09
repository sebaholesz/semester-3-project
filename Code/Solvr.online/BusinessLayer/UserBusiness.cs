using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;

namespace BusinessLayer
{
    public class UserBusiness
    {
        private static readonly UserBusiness _userBusinessInstance = new UserBusiness();
        private readonly IDbUser _dbUser;

        private UserBusiness()
        {
            _dbUser = new DbUser();
        }

        public static UserBusiness GetUserBusiness()
        {
            return _userBusinessInstance;
        }

        public User GetDisplayDataByUserId(string userId)
        {
            return _dbUser.GetDisplayDataByUserId(userId);
        }

        public string GetUserUsername(string userId)
        {
            return _dbUser.GetUserUsername(userId);
        }
        public string GetUserName(string userId)
        {
            return _dbUser.GetUserName(userId);
        }

        public bool CheckIfUserExists(string userId)
        {
            return _dbUser.CheckIfUserExists(userId);
        }

        //public List<User> GetAllUsers()
        //{
        //    return dbUser.GetAllUsers();
        //}

        //public int InsertUser(User user)
        //{
        //    return dbUser.InsertUser(user);
        //}

        //public int UpdateUser(User user, int id)
        //{
        //    return dbUser.UpdateUser(user, id);
        //}

        //public User GetUserById(int id)
        //{
        //    return dbUser.GetUserById(id);
        //}

        //public int DeleteUser(int id)
        //{
        //    return dbUser.DeleteUser(id);
        //}
    }
}
