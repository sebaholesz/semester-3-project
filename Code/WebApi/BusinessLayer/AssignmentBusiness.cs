using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AssignmentBusiness
    {
        DbAssignmentIF dbAssignment = new DbAssignment();

        public List<Assignment> GetAllAssignments()
        {
            return dbAssignment.GetAllAssignments();
        }
        public Boolean CreateAssignment(Assignment assignment)
        {
            return dbAssignment.CreateAssignment(assignment);
        }
    }
}
