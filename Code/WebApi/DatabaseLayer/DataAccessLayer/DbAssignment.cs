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
            this._db = new SqlConnection("Data Source = hildur.ucn.dk; Initial Catalog = dmaj0919_1081479; User ID = dmaj0919_1081479; Password=Password1!;");
        }

        public bool CreateAssignment(Assignment assignment)
        {
            try
            {
                //this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                this._db.Execute(@"Insert into [dbo].[Assignment](description, price, deadline, anonymous) values (@description, @price, @deadline, @anonymous)",
                    new {description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous });
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
