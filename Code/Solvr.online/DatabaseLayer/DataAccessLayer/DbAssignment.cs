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
                        @"Insert into [dbo].[Assignment](title,description, price, postDate, deadline, anonymous, academicLevel, subject, isActive, userId) values (@title, @description, @price, @postDate, @deadline, @anonymous, @academicLevel, @subject, @isActive, @userId); SELECT SCOPE_IDENTITY()",
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
                            isActive = true,
                            userId = assignment.UserId
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
                throw e;
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
                throw e;
            }
        }

        public List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=1 and not userId=@userId", new {userId = userId}).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=0 and not userId=@userId", new {userId = userId}).ToList();
            } catch (SqlException e)
            {
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
        }

        public bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId) 
        {
            try
            {
                List<string> allSolversForAssignment =_db.Query<string>("Select [userId] from [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = asignmentId }).ToList();
                return allSolversForAssignment.Contains(userId);
            }
            catch (SqlException e)
            {
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
        }

        public List<Assignment> GetAllAssignmentsForUser(string userId)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where userId=@userId", new { userid = userId }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllAssignmentsSolvedByUser(string userId)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where assignmentId = (Select assignmentId from [dbo].[Solution] where userId=@userId)", new { userId = userId }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
