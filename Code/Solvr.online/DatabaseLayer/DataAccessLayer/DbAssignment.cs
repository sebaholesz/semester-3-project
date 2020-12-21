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
            using (var transaction = _db.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    User userConcurrencyInfo = _db.QueryFirst<User>("Select [Credit],[ConcurrencyStamp] from [Identity].[User] where Id=@userId", new { userId = assignment.UserId }, transaction: transaction);

                    int returni = _db.Execute(@"Update [Identity].[User] set credit=@credit WHERE Id = @userId AND ConcurrencyStamp = @ConcurrencyStamp",
                        new { credit = userConcurrencyInfo.Credit - assignment.Price, userId = assignment.UserId, ConcurrencyStamp = userConcurrencyInfo.ConcurrencyStamp }, transaction: transaction);
                    if (returni == 1)
                    {
                        string newGuid = Guid.NewGuid().ToString();
                        int generated = _db.Execute(@"Update [Identity].[User] set ConcurrencyStamp=@concurrencystamp WHERE Id = @userId", new { userId = assignment.UserId, concurrencystamp = newGuid }, transaction: transaction);
                        if (generated == 1)
                        {
                            int lastUsedId = _db.ExecuteScalar<int>(
                                @"Insert into [dbo].[Assignment](title,description, price, postDate, deadline, anonymous, academicLevel, subject, isActive, userId) output inserted.assignmentId values (@title, @description, @price, @postDate, @deadline, @anonymous, @academicLevel, @subject, @isActive, @userId)",
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
                                }, transaction: transaction);

                            if (lastUsedId > 0)
                            {
                                if (assignment.AssignmentFile != null)
                                {
                                    int noOfRowsAffectedForFileInsert = _db.Execute(
                                        @"Insert into [dbo].[AssignmentFile] (assignmentId, assignmentFile) values (@assignmentId, @assignmentFile)",
                                        new { assignmentId = lastUsedId, assignmentFile = assignment.AssignmentFile }, transaction: transaction);

                                    if (noOfRowsAffectedForFileInsert == 1)
                                    {
                                        transaction.Commit();
                                        _db.Close();
                                        return lastUsedId;
                                    }
                                }
                                else
                                {
                                    transaction.Commit();
                                    _db.Close();
                                    return lastUsedId;
                                }
                            }
                        }
                    }
                    transaction.Rollback();
                    _db.Close();
                    return -1;
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    _db.Close();
                    throw e;
                }
            }
        }

        public byte[] GetFileFromDB(int assignmentId)
        {
            try
            {
                byte[] fileData = _db.QueryFirst<byte[]>("select assignmentFile from [dbo].[AssignmentFile] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
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

        public List<Assignment> GetAssignmentsByPage(int start)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] Where isActive=1  Order By [postDate] Desc offset @start rows fetch next 12 rows only", new {start = start}).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllActiveAssignmentsNotPostedByUserPage(string userId, int start)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] Where not userId=@userId and isActive=1  Order By [postDate] Desc offset @start rows fetch next 12 rows only", new { userId=userId, start = start }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllAssignmentsForUserPage(string userId, int start)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] Where userId=@userId Order By [postDate] Desc offset @start rows fetch next 12 rows only", new { userId = userId, start = start }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int GetAssignmentsCount()
        {
            try
            {
                return _db.QueryFirst<int>("Select Count(*) from[dbo].[Assignment] where isActive = 1");
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        
        public int GetAssignmentsCountNotByUser(string userId)
        {
            try
            {
                return _db.QueryFirst<int>("Select Count(*) from[dbo].[Assignment] where not userid=@userId and isActive = 1", new { userId = userId});
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int GetAssignmentsCountForUser(string userId)
        {
            try
            {
                return _db.QueryFirst<int>("Select Count(*) from[dbo].[Assignment] where userid=@userId", new { userId = userId });
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
                byte[] inputTimestamp = assignment.Timestamp;
                int returni =  _db.Execute(@"Update [dbo].[Assignment] set title=@title, description=@description, price=@price, deadline=@deadline, anonymous=@anonymous, academicLevel=@academicLevel, subject=@subject WHERE assignmentId = @assignmentId AND timestamp = @timestamp",
                    new { title = assignment.Title, assignmentId = assignmentId, description = assignment.Description, price = assignment.Price, deadline = assignment.Deadline, anonymous = assignment.Anonymous, academicLevel = assignment.AcademicLevel, subject = assignment.Subject, timestamp=inputTimestamp });
                return returni;
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

        public int MakeAssignmentActive(int assignmentId)
        {
            try
            {
                return _db.Execute("Update [dbo].[Assignment] set isActive=1 where assignmentId=@assignmentId", new { assignmentId = assignmentId });
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
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where assignmentId in (Select assignmentId from [dbo].[Solution] where userId=@userId) Order By [postDate] Desc", new { userId = userId }).ToList();
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

        public List<Assignment> GetAllActiveAssignmentsNotPostedByUser(string userId)
        {
            try
            {
                return _db.Query<Assignment>("Select * from [dbo].[Assignment] where not userId=@userId and isActive=1", new { userId = userId }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckIfHasAcceptedSolution(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<bool>("SELECT COUNT(solutionId) FROM [dbo].[Solution] where assignmentId=@assignmentId and accepted=1", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
