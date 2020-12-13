using BusinessLayer.Validation;
using ModelLayer;
using NUnit.Framework;
using System;

namespace SolvrOnlineUnitTests
{
    public class TestSolutionInputValidation
    {
        private SolutionInputValidation solutionInputValidation;

        public TestSolutionInputValidation()
        {
            solutionInputValidation = new SolutionInputValidation();
        }

        [Test]
        public void TestAssignmentDescriptionInputIncorrectTooShort()
        {
            Solution solution = new Solution();

            solution.Anonymous = false;
            solution.Description = "";
            solution.SolutionRating = 3.4m;
            solution.Timestamp = DateTime.UtcNow;
            solution.UserId = "12";
            solution.AssignmentId = 7;

            Assert.IsFalse(solutionInputValidation.CheckInput(solution));
        }

        [Test]
        public void TestAssignmentDescriptionInputIncorrectTooLong()
        {

            #region tooLongDescription
            string tooLongDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque quis eros scelerisque, posuere orci eu, gravida libero. Aliquam placerat nulla a iaculis suscipit. Mauris accumsan pharetra nisi, posuere semper massa venenatis eu. Sed hendrerit auctor diam, vel vestibulum enim mollis vitae. Aenean luctus, dolor ut suscipit venenatis, sem augue ultricies turpis, non tempus lorem lorem sed felis. Suspendisse interdum tincidunt arcu ut lacinia. Maecenas nisi nisi, ultrices non cursus non, fringilla efficitur orci. Fusce lobortis nisl ultrices dui bibendum, quis ultrices nisl rhoncus. Nullam et ";
            #endregion

            Solution solution = new Solution();

            solution.Anonymous = false;
            solution.Description = tooLongDescription;
            solution.SolutionRating = 3.4m;
            solution.Timestamp = DateTime.UtcNow;
            solution.UserId = "12";
            solution.AssignmentId = 7;

            Assert.IsFalse(solutionInputValidation.CheckInput(solution));
        }
    }
}
