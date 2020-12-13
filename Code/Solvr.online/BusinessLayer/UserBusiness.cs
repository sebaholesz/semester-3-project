using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

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
        public int GetUserCredits(string userId)
        {
            return _dbUser.GetUserCredits(userId);
        }
        public int IncreaseUserCredits(int credits, string userId, string stamp)
        {
            int currentCredits = _dbUser.GetUserCredits(userId);
            //int credits = (int)user.Credit;
            return _dbUser.UpdateUserCredits(credits + currentCredits, userId, stamp);
        }
        public int DecreaseUserCredits(int credits, string userId, string stamp)
        {
            int currentCredits = _dbUser.GetUserCredits(userId);
            //int credits = (int)user.Credit;
            //string stamp = user.ConcurrencyStamp;
            return _dbUser.UpdateUserCredits(currentCredits - credits, userId, stamp);
        }
        public string GetUserName(string userId)
        {
            return _dbUser.GetUserName(userId);
        }

        public bool CheckIfUserExists(string userId)
        {
            return _dbUser.CheckIfUserExists(userId);
        }

        public List<User> GetAllUsers()
        {
            return _dbUser.GetAllUsers();
        }

        public string GetUserConcurrencyStamp(string userId)
        {
            return _dbUser.GetUserConcurrencyStamp(userId);
        }

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
