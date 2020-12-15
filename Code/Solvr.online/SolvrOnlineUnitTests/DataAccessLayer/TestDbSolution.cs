using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseLayer;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using NUnit.Framework;
using Utility.HildurConnection;

namespace SolvrOnlineUnitTests
{
    public class TestDbSolution
    {
        private readonly IDbConnection _db;
        private IDbSolution dbS;
        private IDbAssignment dba;
        private IDbUser dbu;
        private int assignmentForTestingId;
        private int assignmentForTestingId2;


        public TestDbSolution()
        {
            _db = new SqlConnection(HildurConnectionString.ConnectionString);
            dbS = new DbSolution();
            dba = new DbAssignment();
            dbu = new DbUser();
        }

        [SetUp]
        public void SetUp()
        {
            List<User> users = dbu.GetAllUsers();
            DateTime now = DateTime.UtcNow;
            Assignment assignment = new Assignment();

            assignment.Title = "Test title";
            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.PostDate = now;
            assignment.Deadline = now.AddDays(5);
            assignment.Anonymous = true;
            assignment.AcademicLevel = "University";
            assignment.Subject = "Computer Science";
            assignment.UserId = users[0].Id;

            assignmentForTestingId = dba.CreateAssignment(assignment);
        }


        [Test]
        public void TestConnection()
        {
            _db.Open();
            Console.WriteLine(_db.State);

            Assert.AreEqual(_db.State, ConnectionState.Open);
            _db.Close();

            Console.WriteLine(_db.State);

        }


        [Test]
        public void TestCreateSolution()
        {
            List<User> users = dbu.GetAllUsers();
            int lengthBefore = dbS.GetAllSolutions().Count;

            Solution solution = new Solution();
            solution.AssignmentId = assignmentForTestingId;
            solution.UserId = users[0].Id;
            solution.Description = "Test description";
            solution.Timestamp = DateTime.UtcNow;
            solution.SolutionRating = 3.6M;
            solution.Anonymous = true;

            int returnedId = dbS.CreateSolution(solution);

            int lengthAfter = dbS.GetAllSolutions().Count;

            Assert.IsTrue((lengthAfter > lengthBefore) && (returnedId>0));
        }

        [Test]
        public void TestCreateSolutionFailWhenDayTimeWrongMultipleTimes()
        {
            List<User> users = dbu.GetAllUsers();

            List<int> resultsArray = new List<int>();

            for (int i = 0; i < 20; i++)
            {
                DateTime now = DateTime.UtcNow;
                Assignment assignment = new Assignment();

                assignment.Title = "Test title";
                assignment.Description = "Assignment description";
                assignment.Price = 10;
                assignment.PostDate = now;
                assignment.Deadline = now.AddDays(5);
                assignment.Anonymous = true;
                assignment.AcademicLevel = "University";
                assignment.Subject = "Computer Science";
                assignment.UserId = users[0].Id;

                assignmentForTestingId2 = dba.CreateAssignment(assignment);

                Solution solution = new Solution();
                solution.AssignmentId = assignmentForTestingId;
                solution.UserId = users[0].Id;
                solution.Description = "Test description 1";
                solution.Timestamp = new DateTime(2017, 1, 18);
                solution.SolutionRating = 3.6M;
                solution.Anonymous = true;

                Solution solutionEarlyDateTime = new Solution();
                solutionEarlyDateTime.AssignmentId = assignmentForTestingId;
                solutionEarlyDateTime.UserId = users[0].Id;
                solutionEarlyDateTime.Description = "Test description 3";
                solutionEarlyDateTime.Timestamp = new DateTime(2017, 1, 17);
                solutionEarlyDateTime.SolutionRating = 3.6M;
                solutionEarlyDateTime.Anonymous = true;

                dbS.CreateSolution(solution);
                int answer1 = Convert.ToInt32(dbS.CreateSolution(solution) == -1);
                int answer2 = Convert.ToInt32(dbS.CreateSolution(solutionEarlyDateTime) == -1);

                resultsArray.Add(answer1);
                resultsArray.Add(answer2);

                dba.DeleteAssignment(assignmentForTestingId2);
            }
            CollectionAssert.DoesNotContain(resultsArray, 0);
        }


        [Test]
        public void TestGetAll()
        {
            List<Solution> solutions = dbS.GetAllSolutions();

            Assert.AreEqual(solutions.GetType(), typeof(List<Solution>));
        }

        [Test]
        public void TestGetBySolutionId()
        {
            List<Solution> solutions = dbS.GetAllSolutions();
            
            int id = solutions[0].SolutionId;

            Solution solution = dbS.GetBySolutionId(id);
            Assert.AreEqual(solution.SolutionId, id);
        }

        [Test]
        public void TestGetByAssignmentId()
        {

            List<User> users = dbu.GetAllUsers();

            #region Arrange
            Solution solution = new Solution();
            solution.AssignmentId = assignmentForTestingId;
            solution.UserId = users[0].Id;
            solution.Description = "Test description 1";
            solution.Timestamp = DateTime.UtcNow;
            solution.SolutionRating = 3.6M;
            solution.Anonymous = true;

            Solution solution2 = new Solution();
            solution2.AssignmentId = assignmentForTestingId;
            solution2.UserId = users[0].Id;
            solution2.Description = "Test description 2";
            solution2.Timestamp = DateTime.UtcNow.AddMinutes(1);
            solution2.SolutionRating = 3.6M;
            solution2.Anonymous = true;

            Solution solution3 = new Solution();
            solution3.AssignmentId = assignmentForTestingId;
            solution3.UserId = users[0].Id;
            solution3.Description = "Test description 3";
            solution3.Timestamp = DateTime.UtcNow.AddMinutes(2);
            solution3.SolutionRating = 3.6M;
            solution3.Anonymous = true;

            dbS.CreateSolution(solution);
            dbS.CreateSolution(solution2);
            dbS.CreateSolution(solution3);
            #endregion
            List<Solution> solutionsByAssignment = dbS.GetSolutionsByAssignmentId(assignmentForTestingId);

            Assert.IsTrue(solutionsByAssignment.Count > 0);
            if (solutionsByAssignment.Count > 0)
            {
                foreach (var sol in solutionsByAssignment)
                {
                    Assert.AreEqual(sol.AssignmentId, assignmentForTestingId);
                }
            }
        }

        [Test]
        public void TestDelete()
        {
            List<User> users = dbu.GetAllUsers();

            #region Arrange
            Solution solution = new Solution();
            solution.AssignmentId = assignmentForTestingId;
            solution.UserId = users[0].Id;
            solution.Description = "Test description 1";
            solution.Timestamp = DateTime.UtcNow;
            solution.SolutionRating = 3.6M;
            solution.Anonymous = true;

            Solution solution2 = new Solution();
            solution2.AssignmentId = assignmentForTestingId;
            solution2.UserId = users[0].Id;
            solution2.Description = "Test description 2";
            solution2.Timestamp = DateTime.UtcNow.AddMinutes(1);
            solution2.SolutionRating = 3.6M;
            solution2.Anonymous = true;

            Solution solution3 = new Solution();
            solution3.AssignmentId = assignmentForTestingId;
            solution3.UserId = users[0].Id;
            solution3.Description = "Test description 3";
            solution3.Timestamp = DateTime.UtcNow.AddMinutes(2);
            solution3.SolutionRating = 3.6M;
            solution3.Anonymous = true;

            dbS.CreateSolution(solution);
            dbS.CreateSolution(solution2);
            dbS.CreateSolution(solution3);
            #endregion
            List<Solution> solutions = dbS.GetSolutionsByAssignmentId(assignmentForTestingId);
            int length = solutions.Count;

            int solutionId = solutions[length-1].SolutionId;

            int countBefore = dbS.GetSolutionsByAssignmentId(assignmentForTestingId).Count;
            dbS.DeleteSolution(solutionId);
            int countAfter = dbS.GetSolutionsByAssignmentId(assignmentForTestingId).Count;
            Assert.AreNotEqual(countBefore, countAfter);
        }
    }
}
