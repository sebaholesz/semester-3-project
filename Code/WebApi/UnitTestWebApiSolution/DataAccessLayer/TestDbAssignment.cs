using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace UnitTestWebApiSolution
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IDbConnection _db;

        public UnitTest1()
        {
            _db = new SqlConnection("Data Source = hildur.ucn.dk; Initial Catalog = dmaj0919_1081479; User ID = dmaj0919_1081479; Password=Password1!;");
        }

        [TestMethod]
        public void TestCreateAssignment()
        {
            DbAssignmentIF dba = new DbAssignment();
            Assignment assignment = new Assignment();

            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Deadline = DateTime.Now;
            assignment.Anonymous = true;

            dba.CreateAssignment(assignment);

            Assert.IsTrue(dba.GetAllAssignments().Count > 0);
        }



        [TestMethod]
        public void TestGetAll()
        {
            DbAssignmentIF dba = new DbAssignment();
            List<Assignment> kokot = dba.GetAllAssignments();

            Assert.AreEqual(kokot.GetType(), typeof(List<Assignment>));
        } 
    }
}
