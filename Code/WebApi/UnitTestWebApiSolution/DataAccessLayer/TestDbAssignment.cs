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
        private DbAssignmentIF dba;

        public UnitTest1()
        {
            _db = new SqlConnection("Data Source = hildur.ucn.dk; Initial Catalog = dmaj0919_1081479; User ID = dmaj0919_1081479; Password=Password1!;");
            dba = new DbAssignment();
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
        public void TestCreateAssignment()
        {
            Assignment assignment = new Assignment();

            //assignment.Description = "Assignment description";
            //assignment.Price = 10;
            //assignment.Deadline = DateTime.Now;
            //assignment.Anonymous = true;

            //dba.CreateAssignment(assignment);


            Assert.IsTrue(dba.GetAllAssignments().Count > 0);
        }



        [TestMethod]
        public void TestGetAll()
        {
            List<Assignment> listOfItems = dba.GetAllAssignments();

            Assert.AreEqual(listOfItems.GetType(), typeof(List<Assignment>));
        }

        [TestMethod]
        public void TestGetById()
        {
            int id = 7;

            Assignment assignment = dba.GetByAssignmentId(id);
            Console.WriteLine(""+assignment.AssignmentId, assignment.Description);

            Assert.AreEqual(assignment.AssignmentId, id);
        }

        [TestMethod]
        public void TestUpdate()
        {
            int id = 12;

            Assignment assignment = dba.GetByAssignmentId(id);
            Console.WriteLine(assignment.Description, assignment.Author, assignment.Price);

            Assignment assignment1 = new Assignment();
            assignment1.Description = "kokot";
            assignment1.Deadline = DateTime.Now;


            dba.UpdateAssignment(assignment1, id);

            assignment = dba.GetByAssignmentId(id);
            Console.WriteLine(assignment.Description, assignment.Author, assignment.Price);

            Assert.AreEqual(assignment.Description, "kokot");


        }

        [TestMethod]
        public void TestDelete()
        {
            int id = 12;
            dba.DeleteAssignment(id);
            Assignment nie = dba.GetByAssignmentId(id);
            Assert.AreEqual(nie, null);
        }
    }
}
