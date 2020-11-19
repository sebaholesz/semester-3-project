using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer;

namespace UnitTestWebApiSolution
{ 
    [TestClass]
    public class TestDbSolution
    {
        private readonly IDbConnection _db;
        private DbSolutionIF dbS;

        public TestDbSolution()
        {
            _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            dbS = new DbSolution();
        }

        [TestMethod]
        public void TestConnection()
        {
            _db.Open();
            Console.WriteLine(_db.State);

            Assert.AreEqual(_db.State, ConnectionState.Open);
            _db.Close();

            Console.WriteLine(_db.State);

        }


        [TestMethod]
        public void TestCreateSolution()
        {
            
            Solution solution = new Solution();
            solution.AssignmentId = 5;
            solution.UserId = 12;
            solution.Description = "Test description";
            solution.Timestamp = DateTime.Now;
            solution.SolutionRating = 3.6M;
            solution.Anonymous = true;

            dbS.CreateSolution(solution);

            Assert.IsTrue(dbS.GetAllSolutions().Count > 0);
           
        }



        [TestMethod]
        public void TestGetAll()
        {
            List<Solution> listOfItems = dbS.GetAllSolutions();

            Assert.AreEqual(listOfItems.GetType(), typeof(List<Solution>));
        }

        [TestMethod]
        public void TestGetBySolutionId()
        {
            int id = 7;

            Solution solution = dbS.GetBySolutionId(id);
            Assert.AreEqual(solution.SolutionId, id);
        }

        [TestMethod]
        public void TestGetByAssignmentId()
        {
            int id = 2;

            List<Solution> solutions = dbS.GetSolutionsByAssignmentId(id);

            Assert.IsTrue(solutions.Count > 0);
            if (solutions.Count > 0)
            {
                foreach (var sol in solutions)
                {
                    Assert.AreEqual(sol.AssignmentId, id);
                }
            }
        }

        [TestMethod]
        public void TestDelete()
        {
            int id = 8;
            int countBefore = dbS.GetAllSolutions().Count;
            dbS.DeleteSolution(id);
            int countAfter = dbS.GetAllSolutions().Count;
            Assert.AreNotEqual(countBefore, countAfter);
        }
    }
}
