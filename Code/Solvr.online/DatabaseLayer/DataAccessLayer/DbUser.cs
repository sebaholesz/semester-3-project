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

        public int UpdateUserCredits(int credit, string userId, string concurrencyStamp)
        {
            //mayeb should be in a transaction
            try
            {
                
                int returni = _db.Execute(@"Update [Identity].[User] set credit=@credit WHERE Id = @userId AND ConcurrencyStamp = @ConcurrencyStamp", new { credit = credit ,userId = userId, ConcurrencyStamp = concurrencyStamp });
                if (returni > 0)
                {
                    int generated = this.GenerateNewConcurrencyStamp(userId);
                    if (generated > 0)
                    {
                        return returni;
                    }
                }

                return -1;
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

        public string GetUserConcurrencyStamp(string userId)
        {
            try
            {
                string result = _db.ExecuteScalar<string>("Select [ConcurrencyStamp] from [Identity].[User] where Id=@userId", new { userId = userId });
                return result;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int GenerateNewConcurrencyStamp(string userId)
        {
            try
            {
                string newGuid = Guid.NewGuid().ToString();
                int result = _db.Execute("Update [Identity].[User] set ConcurrencyStamp=@concurrencystamp WHERE Id = @userId", new { userId = userId, concurrencystamp = newGuid });
                return result;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        //public User GetUserById(int id)
        //{
        //    try
        //    {
        //        return this._db.QueryFirst<User>("Select * from [dbo].[User] where userId=@userId", new { userId = id });
        //    }
        //    catch (SqlException e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //        return null;
        //    }
        //}

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
