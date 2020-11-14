using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbAcademicLevel : DbAcademicLevelIF
    {
        private readonly IDbConnection db;

        public DbAcademicLevel()
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        public List<AcademicLevel> GetAllAcademicLevels()
        {
            return db.Query<AcademicLevel>("select * from [dbo].[AcademicLevel]").ToList();
        }
    }
}
