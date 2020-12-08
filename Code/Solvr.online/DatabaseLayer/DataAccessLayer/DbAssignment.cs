using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utility.HildurConnection;

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

        public byte[] GetFileFromDB(int id)
        {
            //TODO create a default path for users to download file
            try
            {
                byte[] fileData = _db.QueryFirst<byte[]>("select assignmentFile from [dbo].[AssignmentFile] where assignmentId=@assignmentId", new { assignmentId = id });
                return fileData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllAssignments()
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] Order By [postDate] Desc").ToList();
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=1 and not userId=@userId Order By [postDate] Desc", new {userId = userId}).ToList();
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=0 and not userId=@userId Order By [postDate] Desc", new {userId = userId}).ToList();
            } catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllActiveAssignments()
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=1 Order By [postDate] Desc").ToList();
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where isActive=0 Order By [postDate] Desc").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public Assignment GetByAssignmentId(int assignmentId)
        {
            try
            {
                // TODO handle getting "empty" ids
                return _db.QueryFirst<Assignment>("Select * from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        
        public bool CheckIfAssignmentIsStillActive(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<bool>("Select [isActive] from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
            }
            catch (Exception e)
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

        public int UpdateAssignment(Assignment assignment, int assignmentId)
        {
            try
            {
                return _db.Execute(@"Update [dbo].[Assignment] set title=@title, description=@description, price=@price, postDate=@postDate, deadline=@deadline, anonymous=@anonymous, academicLevel=@academicLevel, subject=@subject WHERE assignmentId = @assignmentId",
                    new { title = assignment.Title, assignmentId = assignmentId, description = assignment.Description, price = assignment.Price, postDate = assignment.PostDate, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int MakeAssignmentInactive(int assignmentId)
        {
            try
            {
                return _db.Execute("Update [dbo].[Assignment] set isActive=0 where assignmentId=@assignmentId", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int MakeAssignmentActive(int id)
        {
            try
            {
                return _db.Execute("Update [dbo].[Assignment] set isActive=1 where assignmentId=@assignmentId", new { assignmentId = id });
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

        public int DeleteAssignment(int assignmentId)
        {
            try
            {
                return _db.Execute("DELETE FROM [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where userId=@userId Order By [postDate] Desc", new { userid = userId }).ToList();
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where assignmentId = (Select assignmentId from [dbo].[Solution] where userId=@userId) Order By [postDate] Desc", new { userId = userId }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public string GetAuthorUserId(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<string>("Select userId from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
