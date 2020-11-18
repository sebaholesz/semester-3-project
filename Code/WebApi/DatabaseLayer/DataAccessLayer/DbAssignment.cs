using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbAssignment : DbAssignmentIF
    {
        private readonly IDbConnection db;

        public DbAssignment()
        {
            db = new SqlConnection("Data Source = hildur.ucn.dk; Initial Catalog = dmaj0919_1081479; User ID = dmaj0919_1081479; Password=Password1!;");
        }

        public int CreateAssignment(Assignment assignment)
        {
            try
            {
                int numberOfRowsAffected = db.Execute(@"Insert into [dbo].[Assignment](title,description, price, deadline, anonymous, academicLevel, subject, isActive) values (@title, @description, @price, @deadline, @anonymous, @academicLevel, @subject, @isActive)",
                    new { title = assignment.Title, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject, isActive = true});
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
            //TODO NEED TO ADD GETALLACTIVEASSIGNMENTS/GETALLNOTACTIVEASSIGNMENTS
            return db.Query<Assignment>("Select * from [dbo].[Assignment]").ToList();
        }

        public Assignment GetByAssignmentId(int id)
        {
            try
            {
                return db.QueryFirst<Assignment>("Select * from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
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
                int numberOfRowsAffected = db.Execute(@"Update [dbo].[Assignment] set title=@title, description=@description, price=@price, deadline=@deadline, anonymous=@anonymous, academicLevel=@academicLevel, subject=@subject WHERE assignmentId = @assignmentId",
                    new { title = assignment.Title, assignmentId = id, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject });
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
                return db.Execute("Delete from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }

        public List<string> GetAllAcademicLevels()
        {
            return db.Query<string>("select * from [dbo].[AcademicLevel]").ToList();
        }

        public List<string> GetAllSubjects()
        {
            return db.Query<string>("SELECT * FROM [dbo].[Subject]").ToList();
        }
    }
}
