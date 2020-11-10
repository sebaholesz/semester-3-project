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
    public class DbAssignment : DbAssignmentIF
    {
        private IDbConnection _db;

        public DbAssignment()
        {
            this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        public bool CreateAssignment(Assignment assignment)
        {
            try
            {
                //this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                this._db.Execute(@"Insert into [dbo].[Assignment](assignmentId, description, price, deadline, anonymous) values (@assignmentId, @description, @price, @deadline, @anonymous)",
                    new { assignmentId = assignment.AssignmentId, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous });
                return true;

            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<Assignment> GetAllAssignments()
        {
           // this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return this._db.Query<Assignment>("Select * from [dbo].[Assignment]").ToList();
        }
    }
}
