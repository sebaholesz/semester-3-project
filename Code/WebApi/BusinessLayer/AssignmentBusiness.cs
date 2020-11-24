using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AssignmentBusiness
    {
        private readonly DbAssignmentIF dbAssignment;
        private AssignmentInputValidation assignmentValidation;

        public AssignmentBusiness()
        {
            dbAssignment = new DbAssignment();
            assignmentValidation = new AssignmentInputValidation();

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
            if (assignmentValidation.CheckInput(assignment))
            {
                return dbAssignment.CreateAssignment(assignment);
            }
            return -1;

        }

        public Assignment GetByAssignmentId(int id)
        {
            return dbAssignment.GetByAssignmentId(id);
        }

        public int UpdateAssignment(Assignment assignment, int id)
        {
            //TODO validators 
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
