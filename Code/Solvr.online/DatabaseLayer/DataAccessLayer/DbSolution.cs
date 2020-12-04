using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
            // TODO ADD SOLUTION FILE / 
            try
            {
                List<Solution> solutionsBefore = GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthBefore = solutionsBefore.Count;

                if (queueLengthBefore > 0)
                {
                    if (DateTime.Compare(solution.Timestamp, solutionsBefore[queueLengthBefore - 1].Timestamp) <= 0)
                    {
                        return -1;
                    }
                }

                //check if this assignment wasnt done by the same user and that he hasnt already solved this one
                //can probably be done in a more DRY and clean way, but will do for now
                //TODO refactor
                List<string> userIdsThatSolvedThisAssignment = _db.Query<string>("Select [userId] from [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = solution.AssignmentId }).ToList();

                if(userIdsThatSolvedThisAssignment.Contains(solution.UserId))
                {
                    return -2;
                }

                string assignmentAuthorId = _db.QueryFirst<string>("Select [userId] from [dbo].[Assignment] where assignmentId=@assignmentId", new { assignmentId = solution.AssignmentId });

                if (assignmentAuthorId.Equals(solution.UserId))
                {
                    return -3;
                }

                List<Solution> solutionsAfter = GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthAfter = solutionsAfter.Count;

                if (queueLengthBefore == queueLengthAfter)
                {
                    _db.Open();
                    using (var transaction = _db.BeginTransaction())
                    {
                        try
                        {
                            int lastUsedId = _db.ExecuteScalar<int>(
                                @"INSERT INTO [dbo].[Solution](assignmentId, description, timestamp, solutionRating, anonymous, accepted, userId) " +
                                "VALUES (@assignmentId, @description, @timestamp, @solutionRating, @anonymous, 0, @userId); SELECT SCOPE_IDENTITY()",
                                new
                                {
                                    assignmentId = solution.AssignmentId,
                                    description = solution.Description,
                                    timestamp = solution.Timestamp,
                                    solutionRating = solution.SolutionRating,
                                    anonymous = solution.Anonymous,
                                    userId = solution.UserId
                                }, transaction );
                            if (solution.SolutionFile != null)
                            {
                                _db.Execute(@"INSERT INTO [dbo].[SolutionFile](solutionId, solutionFile) values (@solutionId, @solutionFile)",
                                    new { solutionId = lastUsedId, solutionFile = solution.SolutionFile }, transaction);
                            }
                            transaction.Commit();
                            _db.Close();
                        }
                        catch (SqlException e)           
                        {
                            transaction.Rollback();
                            _db.Close();
                            throw e;
                        }

                    }
                    return queueLengthAfter + 1;
                }
                return -1;
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

        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            try
            {
                return _db.Query<Solution>(
                    "SELECT * FROM [dbo].[Solution] where assignmentId = @assignmentId order by timestamp ASC",
                    new {assignmentId = id}).ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public Solution GetBySolutionId(int id)
        {
            try
            {
                return _db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateSolution(Solution solution, int id)
        {
            try
            {
                int numberOfRowsAffected = _db.Execute(@"UPDATE [dbo].[Solution] SET assignmentId = @assignmentId, userId = @userId, description = @description, timestamp = @timestamp, solutionRating = @solutionRating, anonymous = @anonymous WHERE solutionId = @solutionId",
                    new { assignmentId = solution.AssignmentId, userId = solution.UserId, description = solution.Description, timestamp = solution.Timestamp, solutionRating = solution.SolutionRating, anonymous = solution.Anonymous, solutionId = id });
                return numberOfRowsAffected;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public int DeleteSolution(int id)
        {
            try
            {
                return _db.Execute("DELETE FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
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

        public Solution GetSolutionForUserByAssignmentId(string userId, int assignmentId)
        {
            try
            {
                return _db.QueryFirst<Solution>("Select * from [dbo].[Solution] where assignmentId=@assignmentId and userId=@userId", new { assignmentId = assignmentId, userId = userId });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
