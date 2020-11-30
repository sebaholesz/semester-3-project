using Dapper;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DatabaseLayer.DataAccessLayer
{
    public class DbSolution : IDbSolution
    {
        private readonly IDbConnection db;

        public DbSolution()
        {
            db = new SqlConnection(HildurConnectionString.ConnectionString);
        }

        public int CreateSolution(Solution solution)
        {
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

                List<Solution> solutionsAfter = GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthAfter = solutionsAfter.Count;

                if (queueLengthBefore == queueLengthAfter)
                {
                    db.Execute(@"INSERT INTO [dbo].[Solution](assignmentId, userId, description, timestamp, solutionRating, anonymous, accepted) " +
                        "VALUES (@assignmentId, @userId, @description, @timestamp, @solutionRating, @anonymous, 0)",
                   new
                   {
                       assignmentId = solution.AssignmentId,
                       userId = solution.UserId,
                       description = solution.Description,
                       timestamp = solution.Timestamp,
                       solutionRating = solution.SolutionRating,
                       anonymous = solution.Anonymous
                   });
                    return queueLengthAfter + 1;
                }
                else
                {
                    return -1;
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }


        public List<Solution> GetAllSolutions()
        {
            return db.Query<Solution>("SELECT * FROM [dbo].[Solution]").ToList();
        }

        public List<Solution> GetSolutionsTimestampOrderedByAssignmentId(int id)
        {
            return db.Query<Solution>("SELECT * FROM [dbo].[Solution] where assignmentId = @assignmentId order by timestamp ASC", new { assignmentId = id }).ToList();
        }


        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            return db.Query<Solution>("SELECT * FROM [dbo].[Solution] where assignmentId = @assignmentId", new { assignmentId = id }).ToList();
        }

        public Solution GetBySolutionId(int id)
        {
            try
            {
                return db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
            }
            catch (Exception ex)
            {
                if (ex is SqlException || ex is InvalidExpressionException)
                {
                    System.Console.WriteLine(ex.Message);
                }
                return null;
            }
        }

        public int UpdateSolution(Solution solution, int id)
        {
            try
            {
                int numberOfRowsAffected = db.Execute(@"UPDATE [dbo].[Solution] SET assignmentId = @assignmentId, userId = @userId, description = @description, timestamp = @timestamp, solutionRating = @solutionRating, anonymous = @anonymous WHERE solutionId = @solutionId",
                    new { assignmentId = solution.AssignmentId, userId = solution.UserId, description = solution.Description, timestamp = solution.Timestamp, solutionRating = solution.SolutionRating, anonymous = solution.Anonymous, solutionId = id });
                return numberOfRowsAffected;
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }

        public int DeleteSolution(int id)
        {
            try
            {
                return db.Execute("DELETE FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }

        public int ChooseSolution(int solutionId)
        {
            //using(var transaction = db.BeginTransaction())
            //{
            //    db.Open();
            try
            {
                //mark the one solution as accepted
                return db.Execute("UPDATE [dbo].[Solution]  SET accepted=1 WHERE solutionId=@solutionId", new { solutionId = solutionId });
            }
            catch (Exception e)
            {
                throw e;
            }
            //}
        }
    }
}
