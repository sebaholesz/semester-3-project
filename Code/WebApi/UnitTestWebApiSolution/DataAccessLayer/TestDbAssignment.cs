using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer;
using System;

namespace UnitTestWebApiSolution
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreateAssignment()
        {
            DbAssignmentIF dba = new DbAssignment();
            Assignment assignment = new Assignment();
            DbUserIF dbu = new DbUser();

            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Deadline = DateTime.Now;
            assignment.Anonymous = true;

            User user = new User("fdsfds", "fdsfds", "fdsfds", "fdsfds", "fdsfds", "fdsfds");

            dba.CreateAssignment(assignment);
            //dbu.InsertUser(user);
            Assert.IsTrue(dba.GetAllAssignments().Count == 1);
        }
    }
}
