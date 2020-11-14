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
    public class DbSolution : DbSolutionIF
    {
        private IDbConnection _db;

        public DbSolution()
        {
            this._db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        public int CreateSolution(Solution solution)
        {
            try
            {
                return this._db.Execute(@"INSERT INTO [dbo].[Solution](assignmentId, userId, description, timestamp, solutionRating, anonymous) VALUES (@assignmentId, @userId, @description, @timestamp, @solutionRating, @anonymous)",
                    new { assignmentId = solution.AssignmentId, userId = solution.UserId, description = solution.Description, timestamp = solution.Timestamp, solutionRating = solution.SolutionRating, anonymous = solution.Anonymous });
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }


        public List<Solution> GetAllSolutions()
        {
            return this._db.Query<Solution>("SELECT * FROM [dbo].[Solution]").ToList();
        }

        public Solution GetBySolutionId(int id)
        {
            try
            {
                return this._db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
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
                int numberOfRowsAffected = this._db.Execute(@"UPDATE [dbo].[Solution] SET assignmentId = @assignmentId, userId = @userId, description = @description, timestamp = @timestamp, solutionRating = @solutionRating, anonymous = @anonymous WHERE solutionId = @solutionId",
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
                return this._db.Execute("DELETE FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
