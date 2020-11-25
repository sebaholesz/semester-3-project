using Dapper;
using DatabaseLayer.RepositoryLayer;
using Microsoft.Extensions.Configuration;
using ModelLayer;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbAssignment : IDbAssignment
    {
        private readonly string db;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(db);
            }
        }
        public DbAssignment(IConfiguration configuration)
        {
            db = configuration.GetConnectionString("DefaultConnection");
        }

        public int CreateAssignment(Assignment assignment)
        {
            using IDbConnection conn = Connection;
            try
            {
                int numberOfRowsAffected = conn.Execute(@"Insert into [dbo].[Assignment](title,description, price, deadline, anonymous, academicLevel, subject, isActive) values (@title, @description, @price, @deadline, @anonymous, @academicLevel, @subject, @isActive)",
                    new { title = assignment.Title, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject, isActive = true });
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
            using IDbConnection conn = Connection;
            return conn.Query<Assignment>("Select * from [dbo].[Assignment]").ToList();
        }

        public List<Assignment> GetAllActiveAssignments()
        {
            using IDbConnection conn = Connection;
            return conn.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=1").AsList();
        }

        public List<Assignment> GetAllInactiveAssignments()
        {
            using IDbConnection conn = Connection;
            return conn.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=0").AsList();
        }

        public Assignment GetByAssignmentId(int id)
        {
            using IDbConnection conn = Connection;
            try
            {
                // TODO handle getting "empty" ids
                return conn.QueryFirst<Assignment>("Select * from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }

        }
        public int UpdateAssignment(Assignment assignment, int id)
        {
            using IDbConnection conn = Connection;
            try
            {
                int numberOfRowsAffected = conn.Execute(@"Update [dbo].[Assignment] set title=@title, description=@description, price=@price, deadline=@deadline, anonymous=@anonymous, academicLevel=@academicLevel, subject=@subject WHERE assignmentId = @assignmentId",
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
            using IDbConnection conn = Connection;
            try
            {
                return conn.Execute("Delete from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }

        public List<string> GetAllAcademicLevels()
        {
            using IDbConnection conn = Connection;
            return conn.Query<string>("select * from [dbo].[AcademicLevel]").AsList();
        }

        public List<string> GetAllSubjects()
        {
            using IDbConnection conn = Connection;
            return conn.Query<string>("SELECT * FROM [dbo].[Subject]").AsList();
        }
    }
}
