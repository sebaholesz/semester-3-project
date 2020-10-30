using Dapper;
using DataAccessLayer.RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DbUser : DbUserIF
    {
        private IDbConnection _db;

        public List<object> GetAllUsers()
        {
            this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return this._db.Query<object>("Select * from [dbo].[User]").ToList();
        }
    }
}
