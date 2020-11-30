using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbAssignment : IDbAssignment
    {
        private readonly IDbConnection _db;

        public DbAssignment()
        {
            _db = new SqlConnection(HildurConnectionString.ConnectionString);
        }

        public int CreateAssignment(Assignment assignment)
        {
            _db.Open();
            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    int lastUsedId = _db.ExecuteScalar<int>(
                        @"Insert into [dbo].[Assignment](title,description, price, postDate, deadline, anonymous, academicLevel, subject, isActive) values (@title, @description, @price, @postDate, @deadline, @anonymous, @academicLevel, @subject, @isActive); SELECT SCOPE_IDENTITY()",
                        new
                        {
                            title = assignment.Title,
                            description = assignment.Description,
                            price = assignment.Price,
                            postDate = assignment.PostDate,
                            deadline = assignment.Deadline,
                            anonymous = assignment.Anonymous,
                            academicLevel = assignment.AcademicLevel,
                            subject = assignment.Subject,
                            isActive = true
                        }, transaction);
                    if (assignment.AssignmentFile != null)
                    {
                        _db.Execute(
                            @"Insert into [dbo].[AssignmentFile](assignmentId, assignmentFile) values (@assignmentId, @assignmentFile)",
                            new { assignmentId = lastUsedId, assignmentFile = assignment.AssignmentFile }, transaction);
                    }

                    transaction.Commit();
                    _db.Close();
                    return lastUsedId;
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    _db.Close();
                    System.Console.WriteLine(e.Message);
                    return -1;
                }
            }
        }

        public void GetFileFromDB(int id)
        {
            //TODO create a default path for users to download file
            byte[] fileData = _db.QueryFirst<byte[]>("select assignmentFile from [dbo].[AssignmentFile] where assignmentId=@assignmentId", new { assignmentId = id });

            using (MemoryStream ms = new MemoryStream(fileData))
            {
                try
                {
                    FileStream file = new FileStream(@"C:\Users\Lenovo\Desktop\plswork.png", FileMode.Create, FileAccess.Write);
                    ms.WriteTo(file);
                    file.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public List<Assignment> GetAllAssignments()
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment]").ToList();
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Assignment> GetAllActiveAssignments()
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=1").ToList();
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Assignment> GetAllInactiveAssignments()
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=0").ToList();
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        public Assignment GetByAssignmentId(int id)
        {
            try
            {
                // TODO handle getting "empty" ids
                return _db.QueryFirst<Assignment>("Select * from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
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
                int numberOfRowsAffected = _db.Execute(@"Update [dbo].[Assignment] set title=@title, description=@description, price=@price, postDate=@postDate deadline=@deadline, anonymous=@anonymous, academicLevel=@academicLevel, subject=@subject WHERE assignmentId = @assignmentId",
                    new { title = assignment.Title, assignmentId = id, description = assignment.Description, price = assignment.Price, postDate = assignment.PostDate, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject });
                return numberOfRowsAffected;
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return -1;
            }
        }

        public int MakeAssignmentInactive(int id)
        {
            try
            {
                return _db.Execute("Update [dbo].[Assignment] set isActive=0 where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return -1;
            }
        }

        public List<string> GetAllAcademicLevels()
        {
            try
            {
                return _db.Query<string>("select * from [dbo].[AcademicLevel]").ToList();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<string> GetAllSubjects()
        {
            try
            {
                return _db.Query<string>("SELECT * FROM [dbo].[Subject]").ToList();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public int DeleteAssignment(int id)
        {
            try
            {
                return _db.Execute("DELETE * FROM [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
