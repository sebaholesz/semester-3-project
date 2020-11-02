using Dapper;
using DataAccessLayer.RepositoryLayer;
using ModelLayer.User;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessLayer
{
    public class DbUser : DbUserIF
    {
        private IDbConnection _db;

        public List<User> GetAllUsers()
        {
            this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return this._db.Query<User>("Select * from [dbo].[User]").ToList();
        }

        public int InsertUser(User user)
        {
            try
            {
                this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                int numberOfRowsAffected = this._db.Execute(@"Insert into [dbo].[User](username, lastLogin, password, firstName, lastName, email) values (@username, @lastLogin, @password, @firstName, @lastName, @email)",
                    new { username = user.Username, lastlogin = user.LastLogin, password = user.Password, firstName = user.FirstName, lastName = user.LastName, email = user.Email });
                return numberOfRowsAffected;
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
