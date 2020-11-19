using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AssignmentBusiness
    {
        private readonly DbAssignmentIF dbAssignment;

        public AssignmentBusiness()
        {
            dbAssignment = new DbAssignment();
        }

        public List<Assignment> GetAllAssignments()
        {
            return dbAssignment.GetAllAssignments();
        }
        public List<Assignment> GetAllActiveAssignments()
        {
            return dbAssignment.GetAllActiveAssignments();
        }
        public List<Assignment> GetAllInactiveAssignments()
        {
            return dbAssignment.GetAllInactiveAssignments();
        }
        public int CreateAssignment(Assignment assignment)
        {
            return dbAssignment.CreateAssignment(assignment);
        }

        public Assignment GetByAssignmentId(int id)
        {
            return dbAssignment.GetByAssignmentId(id);
        }

        public int UpdateAssignment(Assignment assignment, int id)
        {
            return dbAssignment.UpdateAssignment(assignment, id);
        }

        public int DeleteAssignment(int id)
        {
            return dbAssignment.DeleteAssignment(id);
        }

        public List<string> GetAllAcademicLevels()
        {
            return dbAssignment.GetAllAcademicLevels();
        }

        public List<string> GetAllSubjects()
        {
            return dbAssignment.GetAllSubjects();
        }
    }
}
