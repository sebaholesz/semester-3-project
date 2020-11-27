﻿using Dapper;
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
                List<Solution> solutionsAfter = GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthAfter = solutionsAfter.Count;

                if (queueLengthBefore == queueLengthAfter)
                {
                    _db.Execute(
                        @"INSERT INTO [dbo].[Solution](assignmentId, userId, description, timestamp, solutionRating, anonymous) " +
                        "VALUES (@assignmentId, @userId, @description, @timestamp, @solutionRating, @anonymous)",
                        new
                        {
                            assignmentId = solution.AssignmentId,
                            userId = solution.UserId,
                            description = solution.Description,
                            timestamp = solution.Timestamp,
                            solutionRating = solution.SolutionRating,
                            anonymous = solution.Anonymous
                        });
                    // if(solution.SolutionFile)
                    // add solution file to DB
                    
                    return queueLengthAfter + 1;
                }
                return -1;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return -1;
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
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            try
            {
                return _db.Query<Solution>(
                    "SELECT * FROM [dbo].[Solution] where assignmentId = @assignmentId order by timestamp DESC",
                    new {assignmentId = id}).ToList();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public Solution GetBySolutionId(int id)
        {
            try
            {
                return _db.QueryFirst<Solution>("SELECT * FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
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
                int numberOfRowsAffected = _db.Execute(@"UPDATE [dbo].[Solution] SET assignmentId = @assignmentId, userId = @userId, description = @description, timestamp = @timestamp, solutionRating = @solutionRating, anonymous = @anonymous WHERE solutionId = @solutionId",
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
                return _db.Execute("DELETE FROM [dbo].[Solution] WHERE solutionId=@solutionId", new { solutionId = id });
            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
