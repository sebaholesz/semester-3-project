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
    public class DbSsubject : DbSubjectIF
    {
        private readonly IDbConnection db;

        public DbSsubject()
        {
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        public List<Subject> GetAllSubjects()
        {
            return db.Query<Subject>("SELECT * FROM [dbo].[Subject]").ToList();
        }
    }
}
