using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UnitTestWebApiSolution
{
    [TestClass]
    public class TestDbAssignment
    {
        private readonly IDbConnection _db;
        private DbAssignmentIF dba;

        public TestDbAssignment()
        {
            _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
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
            int length1 = dba.GetAllAssignments().Count;


            Assignment assignment = new Assignment();

            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Title = "kokot";
            assignment.Subject = "History";
            assignment.AcademicLevel = "University";
            assignment.Deadline = DateTime.Now;
            assignment.Anonymous = true;


            dba.CreateAssignment(assignment);


            int length2 = dba.GetAllAssignments().Count;

            Assert.IsTrue(length2 > length1);
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
            List<Assignment> listOfItems = dba.GetAllAssignments();
            int length = listOfItems.Count;

            int id = listOfItems[length - 1].AssignmentId;

            Assignment assignment = dba.GetByAssignmentId(id);
            Console.WriteLine("" + assignment.AssignmentId, assignment.Description);

            Assert.AreEqual(assignment.AssignmentId, id);
        }

        [TestMethod]
        //TODO: REDO
        public void TestUpdate()
        {
            List<Assignment> listOfItems = dba.GetAllAssignments();
            int id = listOfItems[0].AssignmentId;

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
            List<Assignment> listOfItems = dba.GetAllAssignments();
            int id = listOfItems[0].AssignmentId;

            int countBefore = dba.GetAllAssignments().Count;
            dba.DeleteAssignment(id);
            int countAfter = dba.GetAllAssignments().Count;
            Assert.AreNotEqual(countBefore, countAfter);
        }


        [TestMethod]

        public void TestInsertFile()
        {
            Assignment assignment = new Assignment();

            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Title = "tokok";
            assignment.Subject = "History";
            assignment.AcademicLevel = "University";
            assignment.Deadline = DateTime.Now;
            assignment.Anonymous = true;

            int result = dba.CreateAssignmentWithFile(assignment, @"C:\Users\samla\Downloads\kmfdm_99-img1200x1200-1601775397hdvx3m20963.jpg");
            Console.WriteLine(result);

            Assert.IsTrue(result > 0);

        }

        [TestMethod]

        public void TestReadFile()
        {
            dba.GetFileFromDB(37);
        }
    }
}
