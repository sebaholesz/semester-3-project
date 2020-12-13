﻿using DatabaseLayer.DataAccessLayer;
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
            try
            {
                return _userBusinessInstance;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public User GetDisplayDataByUserId(string userId)
        {
            try
            {
                return _dbUser.GetDisplayDataByUserId(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public string GetUserUsername(string userId)
        {
            try
            {
                return _dbUser.GetUserUsername(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public User GetUserByUserName(string userUserName)
        {
            try
            {
                return _dbUser.GetUserByUserName(userUserName);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public User GetUserById(string userId)
        {
            try
            {
                return _dbUser.GetUserById(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public int GetUserCredits(string userId)
        {
            try
            {
                return _dbUser.GetUserCredits(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public int IncreaseUserCredits(int credits, string userId, string stamp)
        {
            try
            {
                int currentCredits = _dbUser.GetUserCredits(userId);
                return _dbUser.UpdateUserCredits(credits + currentCredits, userId, stamp);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int DecreaseUserCredits(int credits, string userId, string stamp)
        {
            try
            {
                int currentCredits = _dbUser.GetUserCredits(userId);
                return _dbUser.UpdateUserCredits(currentCredits - credits, userId, stamp);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public string GetUserName(string userId)
        {
            try
            {
                return _dbUser.GetUserName(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool CheckIfAdminOrModerator(string userUsername)
        {
            try
            {
                string role = _dbUser.GetRoleByUserName(userUsername);

                if (role.Equals("MODERATOR") || role.Equals("ADMIN"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool CheckIfUserExists(string userId)
        {
            try
            {
                return _dbUser.CheckIfUserExists(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
       
        public List<User> GetAllUsers()
        {
            try
            {
                return _dbUser.GetAllUsers();
            }
            catch(Exception e)
            {
                throw e;
            }
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

        public string GetUserConcurrencyStamp(string userId)
        {
            try
            { 
                return _dbUser.GetUserConcurrencyStamp(userId); 
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
