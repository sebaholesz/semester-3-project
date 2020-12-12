using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utility.HildurConnection;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbUser : IDbUser
    {
        private IDbConnection _db;

        public DbUser()
        {
            _db = new SqlConnection(HildurConnectionString.ConnectionString);
        }

        public User GetDisplayDataByUserId(string userId)
        {
            try
            {
                User user = _db.QueryFirst<User>("Select [UserName], [FirstName], [LastName] from [Identity].[User] where id=@userId", new { userId = userId });
                return user;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        
        public string GetUserUsername(string userId)
        {
            try
            {
                string username = _db.QueryFirst<string>("Select [UserName] from [Identity].[User] where id=@userId", new { userId = userId });
                return username;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        
        public int GetUserCredits(string userId)
        {
            try
            {
                int credits = Int32.Parse( _db.QueryFirst<string>("Select [Credit] from [Identity].[User] where Id=@userId", new { userId = userId }));
                return credits;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int UpdateUserCredits(int credit, string userId)
        {
            try
            {
                
                int returni = _db.Execute(@"Update [Identity].[User] set credit=@credit WHERE Id = @userId", new { credit = credit ,userId = userId});

                return returni;
            }
            catch (SqlException e)
            {

                throw e;
            }
        }

        public string GetUserName(string userId)
        {
            try
            {
                string firstName = _db.QueryFirst<string>("Select [FirstName] from [Identity].[User] where id=@userId", new { userId = userId });
                string lastName = _db.QueryFirst<string>("Select [LastName] from [Identity].[User] where id=@userId", new { userId = userId });
                return firstName + " " + lastName;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public bool CheckIfUserExists(string userId)
        {
            try
            {
                return _db.ExecuteScalar<bool>("select count(1) from [Identity].[User] where Id=@userId", new { userId = userId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _db.Query<User>("Select * from [Identity].[User]").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
            
        }

        public string GetUserHashedPasswordByUserName(string userUserName)
        {
            try
            {
                return _db.QueryFirst<string>("Select [PasswordHash] from [Identity].[User] where UserName=@userUserName", new { userUserName = userUserName });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public string GetUserRoleByUserName(string userUserName)
        {
            try
            {
                return _db.QueryFirst<string>("Select [RoleId] from [Identity].[UserRoles] where UserId=(Select [Id] from [Identity].[User] where UserName=@userUserName )", new { userUserName = userUserName });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public User GetUserByUserName(string userUserName)
        {
            try
            {
                return _db.QueryFirst<User>("Select [Id], [UserName], [Email] from [Identity].[User] where UserName=@userUserName", new { userUserName = userUserName });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public User GetUserById(string userId)
        {
            try
            {
                return _db.QueryFirst<User>("Select [Id], [UserName], [Email] from [Identity].[User] where Id=@userId", new { userId = userId });
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
                return _db.ExecuteScalar<bool>("select count(1) from [Identity].[User] where Id=@id and SecurityStamp=@securityStamp", new { id = userToAuthenticate.Id, securityStamp = userToAuthenticate.SecurityStamp });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        //public int InsertUser(User user)
        //{
        //    try
        //    {
        //        this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //        int numberOfRowsAffected = this._db.Execute(@"Insert into [dbo].[User](username, lastLogin, password, firstName, lastName, email) values (@username, @lastLogin, @password, @firstName, @lastName, @email)",
        //            new { username = user.Username, lastlogin = user.LastLogin, password = user.Password, firstName = user.FirstName, lastName = user.LastName, email = user.Email });
        //        return numberOfRowsAffected;
        //    }
        //    catch (SqlException e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //        return 0;
        //    }
        //}

        //public int UpdateUser(User user, int id)
        //{
        //    try
        //    {
        //        this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //        int numberOfRowsAffected = this._db.Execute(@"Update [dbo].[User] set username=@username, lastLogin=@lastLogin, password=@password, firstName=@firstName, lastName=@lastName, email=@email WHERE userId = @userId",
        //            new { userId = id, username = user.Username, lastlogin = user.LastLogin, password = user.Password, firstName = user.FirstName, lastName = user.LastName, email = user.Email });
        //        return numberOfRowsAffected;
        //    }
        //    catch (SqlException e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //        return 0;

        //    }
        //}

        //public User GetUserById(int id)
        //{
        //    try
        //    {
        //        this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //        return this._db.QueryFirst<User>("Select * from [dbo].[User] where userId=@userId", new { userId = id });
        //    }
        //    catch (SqlException e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //        return null;
        //    }
        //}

        //public int DeleteUser(int id)
        //{
        //    try
        //    {
        //        this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //        return this._db.Execute("Delete from [dbo].[User] where userId=@userId", new { userId = id });
        //    }
        //    catch (SqlException e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //        return 0;
        //    }
        //}
    }
}
