﻿using System;
using BusinessLayer.Validation;
using ModelLayer;
using NUnit.Framework;

namespace SolvrOnlineUnitTests
{
    public class TestAssignmentInputValidation
    {
        private AssignmentInputValidation assignmentInputValidation;
        private DateTime deadline = DateTime.UtcNow;

        public TestAssignmentInputValidation()
        {
            assignmentInputValidation = new AssignmentInputValidation();
        }

        [Test]
        public void TestAssignmentInputCorrect()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsTrue(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentTitleInputIncorrectTooShort()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "blob";
            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentTitleInputIncorrectTooLong()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "This title is just too long to pass the test, you will see";
            assignment.Description = "Assignment description";
            assignment.Price = 10;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentDescriptionInputIncorrectTooShort()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "just some title";
            assignment.Description = "";
            assignment.Price = 10;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentDescriptionInputIncorrectTooLong()
        {
            Assignment assignment = new Assignment();

            #region tooLongDescription
            string tooLongDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque quis eros scelerisque, posuere orci eu, gravida libero. Aliquam placerat nulla a iaculis suscipit. Mauris accumsan pharetra nisi, posuere semper massa venenatis eu. Sed hendrerit auctor diam, vel vestibulum enim mollis vitae. Aenean luctus, dolor ut suscipit venenatis, sem augue ultricies turpis, non tempus lorem lorem sed felis. Suspendisse interdum tincidunt arcu ut lacinia. Maecenas nisi nisi, ultrices non cursus non, fringilla efficitur orci. Fusce lobortis nisl ultrices dui bibendum, quis ultrices nisl rhoncus. Nullam et ";
            #endregion

            assignment.Title = "just some title";
            assignment.Description = tooLongDescription;
            assignment.Price = 10;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentPriceInputIncorrectTooLow()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = -1;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentPriceInputIncorrectToHigh()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = 500000000;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentDeadlineInputIncorrectEarlierDateTime()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = 500000000;
            assignment.Deadline = deadline.AddDays(-5);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentAcademicLevelInputIncorrect()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = 500000000;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "blob";
            assignment.Subject = "History";

            Assert.IsFalse(assignmentInputValidation.CheckInput(assignment));
        }

        [Test]
        public void TestAssignmentSubjectInputIncorrect()
        {
            Assignment assignment = new Assignment();

            assignment.Title = "Just some title";
            assignment.Description = "Assignment description";
            assignment.Price = 500000000;
            assignment.Deadline = deadline.AddDays(1);
            assignment.AcademicLevel = "High School First Grade";
            assignment.Subject = "blob";
        }
    }
}
