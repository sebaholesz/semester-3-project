using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

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

        public User GetUserByUserName(string userUserName)
        {
            return _dbUser.GetUserByUserName(userUserName);
        }

        public User GetUserById(string userId)
        {
            return _dbUser.GetUserById(userId);
        }

        public int GetUserCredits(string userId)
        {
            return _dbUser.GetUserCredits(userId);
        }
        public int IncreaseUserCredits(int credits, string userId)
        {
            int currentCredits = _dbUser.GetUserCredits(userId);
            return _dbUser.UpdateUserCredits(credits + currentCredits, userId);
        }
        public int DecreaseUserCredits(int credits, string userId)
        {
            int currentCredits = _dbUser.GetUserCredits(userId);
            return _dbUser.UpdateUserCredits(currentCredits - credits, userId);
        }
        public string GetUserName(string userId)
        {
            return _dbUser.GetUserName(userId);
        }
        public bool CheckIfAdminOrModerator(string userUsername)
        {
            string role = _dbUser.GetRoleByUserName(userUsername);
            if (role.Equals("MODERATOR") || role.Equals("ADMIN"))
            {
                return true;
            }
            return false;
        }
        public bool CheckIfUserExists(string userId)
        {
            return _dbUser.CheckIfUserExists(userId);
        }
       
        public List<User> GetAllUsers()
        {
            return _dbUser.GetAllUsers();
        }
        
        public bool AuthenticateUser(User userToAuthenticate)
        {
            try
            {
                PasswordHasher<User> ph = new PasswordHasher<User>();
                string currentHash = _dbUser.GetUserHashedPasswordByUserName(userToAuthenticate.UserName);
                PasswordVerificationResult pvr = ph.VerifyHashedPassword(userToAuthenticate, currentHash, userToAuthenticate.Password);
                return pvr == PasswordVerificationResult.Success;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool AuthenticateUserWithIdAndSecurityStamp(User userToAuthenticate)
        {
            try
            {
                return _dbUser.AuthenticateUserWithIdAndSecurityStamp(userToAuthenticate);
            }
            catch (Exception e)
            {
                throw e;
            }
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
