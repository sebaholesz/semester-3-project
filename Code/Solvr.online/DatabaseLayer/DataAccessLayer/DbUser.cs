//using Dapper;
//using DatabaseLayer.RepositoryLayer;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;

//namespace DatabaseLayer.DataAccessLayer
//{
//    public class DbUser : DbUserIF
//    {
//        private IDbConnection _db;



//        public List<User> GetAllUsers()
//        {
//            this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
//            return this._db.Query<User>("Select * from [dbo].[User]").ToList();
//        }

//        public int InsertUser(User user)
//        {
//            try
//            {
//                this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
//                int numberOfRowsAffected = this._db.Execute(@"Insert into [dbo].[User](username, lastLogin, password, firstName, lastName, email) values (@username, @lastLogin, @password, @firstName, @lastName, @email)",
//                    new { username = user.Username, lastlogin = user.LastLogin, password = user.Password, firstName = user.FirstName, lastName = user.LastName, email = user.Email });
//                return numberOfRowsAffected;
//            }
//            catch (SqlException e)
//            {
//                System.Console.WriteLine(e.Message);
//                return 0;
//            }
//        }

//        public int UpdateUser(User user, int id)
//        {
//            try
//            {
//                this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
//                int numberOfRowsAffected = this._db.Execute(@"Update [dbo].[User] set username=@username, lastLogin=@lastLogin, password=@password, firstName=@firstName, lastName=@lastName, email=@email WHERE userId = @userId",
//                    new { userId = id, username = user.Username, lastlogin = user.LastLogin, password = user.Password, firstName = user.FirstName, lastName = user.LastName, email = user.Email });
//                return numberOfRowsAffected;
//            }
//            catch (SqlException e)
//            {
//                System.Console.WriteLine(e.Message);
//                return 0;

//            }
//        }

//        public User GetUserById(int id)
//        {
//            try
//            {
//                this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
//                return this._db.QueryFirst<User>("Select * from [dbo].[User] where userId=@userId", new { userId = id });
//            }
//            catch (SqlException e)
//            {
//                System.Console.WriteLine(e.Message);
//                return null;
//            }
//        }

//        public int DeleteUser(int id)
//        {
//            try
//            {
//                this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
//                return this._db.Execute("Delete from [dbo].[User] where userId=@userId", new { userId = id });
//            }
//            catch (SqlException e)
//            {
//                System.Console.WriteLine(e.Message);
//                return 0;
//            }
//        }
//    }
//}
