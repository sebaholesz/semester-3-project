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
    public class DbSolution : IDbSolution
    {
        private readonly IDbConnection _db;

        public DbSolution()
        {
            _db = new SqlConnection(HildurConnectionString.ConnectionString);
        }

        public int CreateSolution(Solution solution)
        {
            try
            {
                int lastUsedId;
                _db.Close();
                _db.Open();
                using (var transaction = _db.BeginTransaction())
                {
                    try
                    {
                        //try if creation was successful
                        lastUsedId = _db.ExecuteScalar<int>(
                                                        @"INSERT INTO [dbo].[Solution](assignmentId, userId, description, timestamp, solutionRating, anonymous, accepted) OUTPUT INSERTED.solutionId " +
                                                        "VALUES (@assignmentId, @userId, @description, @timestamp, @solutionRating, @anonymous, 0)",
                                                        new
                                                        {
                                                            assignmentId = solution.AssignmentId,
                                                            userId = solution.UserId,
                                                            description = solution.Description,
                                                            timestamp = solution.Timestamp,
                                                            solutionRating = solution.SolutionRating,
                                                            anonymous = solution.Anonymous
                                                        }, transaction: transaction);


                        if (solution.SolutionFile != null)
                        {
                            _db.Execute(@"INSERT INTO [dbo].[SolutionFile](solutionId, solutionFile) values (@solutionId, @solutionFile)",
                                new { solutionId = lastUsedId, solutionFile = solution.SolutionFile }, transaction: transaction);
                        }




                        //doesn't actually check if the input was successful
                        if (lastUsedId > 0)
                        {
                            int noOfSolutions = _db.QueryFirst<int>("SELECT COUNT(*) FROM [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = solution.AssignmentId }, transaction: transaction);
                            //check if solution is the first in the queue
                            if (noOfSolutions==1)
                            {
                                transaction.Commit();
                                _db.Close();
                                return lastUsedId;
                            }
                            
                            if (noOfSolutions>1)
                            {
                                Solution lastPostedSolution = _db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] where assignmentId=@assignmentId order by timestamp DESC", new { assignmentId = solution.AssignmentId }, transaction: transaction);
                                //checks if the Id put into the DB really is last
                                if (lastPostedSolution.SolutionId == lastUsedId)
                                {
                                    bool isAssignmentActive = _db.QueryFirst<bool>("Select [isActive] from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = solution.AssignmentId }, transaction: transaction);
                                    //check if assignment of the solution is active
                                    if (isAssignmentActive)
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
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Solution> GetAllSolutions()
        {
            try
            {
                return _db.Query<Solution>("SELECT * FROM [dbo].[Solution]").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public List<Solution> GetSolutionsByAssignmentId(int assignmentId)
        {
            try
            {
                return _db.Query<Solution>(
                    "SELECT * FROM [dbo].[Solution] where assignmentId = @assignmentId order by timestamp ASC",
                    new { assignmentId = assignmentId }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public Solution GetBySolutionId(int solutionId)
        {
            try
            {
                return _db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = solutionId });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Solution GetSolutionByAssignmentId(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<Solution>("Select * from [dbo].[Solution] where assignmentId=@assignmentId ", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int UpdateSolution(Solution solution, int solutionId)
        {
            try
            {
                return _db.Execute(@"UPDATE [dbo].[Solution] SET assignmentId = @assignmentId, userId = @userId, description = @description, timestamp = @timestamp, solutionRating = @solutionRating, anonymous = @anonymous WHERE solutionId = @solutionId",
                    new { assignmentId = solution.AssignmentId, userId = solution.UserId, description = solution.Description, timestamp = solution.Timestamp, solutionRating = solution.SolutionRating, anonymous = solution.Anonymous, solutionId = solutionId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int DeleteSolution(int solutionId)
        {
            try
            {
                return _db.Execute("DELETE FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = solutionId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int ChooseSolution(int solutionId)
        {
            try
            {
                //mark the one solution as accepted
                return _db.Execute("UPDATE [dbo].[Solution]  SET accepted=1 WHERE solutionId=@solutionId", new { solutionId = solutionId });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<string> GetAllSolversForAssignment(int assignmentId)
        {
            try
            {
                return _db.Query<string>("Select userId from [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = assignmentId }).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int GetSolutionsCountByAssignmentId(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<int>("SELECT COUNT(solutionId) FROM [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public Solution GetAcceptedSolutionForAssignment(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<Solution>("Select * from [dbo].[Solution] where assignmentId=@assignmentId and accepted=1", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        public Solution GetSolutionForAssignment(int assignmentId)
        {
            try
            {
                return _db.QueryFirst<Solution>("Select * from [dbo].[Solution] where assignmentId=@assignmentId and accepted=0", new { assignmentId = assignmentId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
    