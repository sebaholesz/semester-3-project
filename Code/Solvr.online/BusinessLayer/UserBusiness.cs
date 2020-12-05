using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;

namespace BusinessLayer
{
    public class UserBusiness
    {
        private readonly IDbUser _dbUser;


        public UserBusiness()
        {
            _dbUser = new DbUser();
        }

        public string GetUserUsername(string userId)
        {
            return _dbUser.GetUserUsername(userId);
        }
        public string GetUserName(string userId)
        {
            return _dbUser.GetUserName(userId);
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
