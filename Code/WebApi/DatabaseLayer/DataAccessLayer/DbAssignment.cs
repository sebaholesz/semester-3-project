using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.Pipes;
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

        public int CreateAssignment(Assignment assignment)
        {
            try
            {
                int numberOfRowsAffected = this._db.Execute(@"Insert into [dbo].[Assignment](description, price, deadline, anonymous) values (@description, @price, @deadline, @anonymous)",
                    new { description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous });
                return numberOfRowsAffected;

            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }

        public List<Assignment> GetAllAssignments()
        {
            return this._db.Query<Assignment>("Select * from [dbo].[Assignment]").ToList();
        }

        public Assignment GetByAssignmentId(int id)
        {
            try
            {
                return this._db.QueryFirst<Assignment>("Select * from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }
        public int UpdateAssignment(Assignment assignment, int id)
        {
            try
            {
                int numberOfRowsAffected = this._db.Execute(@"Update [dbo].[Assignment] set description=@description, price=@price, deadline=@deadline, anonymous=@anonymous WHERE assignmentId = @assignmentId",
                    new { assignmentId = id, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous });
                return numberOfRowsAffected;
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;

            }
        }
        public int DeleteAssignment(int id)
        {
            try
            {
                return this._db.Execute("Delete from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
