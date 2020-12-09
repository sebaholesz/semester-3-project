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
                _db.Open();
                using (var transaction = _db.BeginTransaction())
                {
                    try
                    {
                        lastUsedId = _db.ExecuteScalar<int>(
                            @"INSERT INTO [dbo].[Solution](assignmentId, userId, description, timestamp, solutionRating, anonymous, accepted) " +
                            "VALUES (@assignmentId, @userId, @description, @timestamp, @solutionRating, @anonymous, 0); SELECT SCOPE_IDENTITY()",
                            new
                            {
                                assignmentId = solution.AssignmentId,
                                userId = solution.UserId,
                                description = solution.Description,
                                timestamp = solution.Timestamp,
                                solutionRating = solution.SolutionRating,
                                anonymous = solution.Anonymous
                            }, transaction);
                            
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
                return lastUsedId;
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
                return _db.Query<string>("Select userId from [dbo].[Solution] where assignmentId=@assignmentId", new { assignmentId = assignmentId}).ToList();
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
    }
}
    