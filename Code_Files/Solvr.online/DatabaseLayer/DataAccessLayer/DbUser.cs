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
        
        public User GetUserCredits(string userId)
        {
            try
            {
                User user = _db.QueryFirst<User>("Select [Credit],[ConcurrencyStamp] from [Identity].[User] where Id=@userId", new { userId = userId });
                return user;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int UpdateUserCredits(int credit, string userId, string concurrencyStamp)
        {

            _db.Open();
            using (var transaction = _db.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    int noOfRowsAffectedForUpdateCredit = _db.Execute(@"Update [Identity].[User] set credit=@credit WHERE Id = @userId AND ConcurrencyStamp = @ConcurrencyStamp", new { credit = credit, userId = userId, ConcurrencyStamp = concurrencyStamp }, transaction: transaction);
                    if (noOfRowsAffectedForUpdateCredit == 1)
                    {
                        string newGuid = Guid.NewGuid().ToString();
                        int noOfGUIDGenerated = _db.Execute(@"Update [Identity].[User] set ConcurrencyStamp=@concurrencystamp WHERE Id = @userId", new { userId = userId, concurrencystamp = newGuid }, transaction: transaction);
                        if (noOfGUIDGenerated == 1)
                        {
                            transaction.Commit();
                            _db.Close();
                            return noOfRowsAffectedForUpdateCredit;

                        }
                    }
                    transaction.Rollback();
                    _db.Close();
                    return -1;

                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    _db.Close();
                    throw e;
                }
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

        public string GetRoleByUserName(string userUserName)
        {
            try
            {
                return _db.QueryFirst<string>("Select [NormalizedName] from [Identity].[Role] where Id = (Select [RoleId] from [Identity].[UserRoles] where " +
                    "UserId=(Select [Id] from [Identity].[User] where UserName=@userUserName))", new { userUserName = userUserName });
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
    }
}
