using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AssignmentBusiness
    {
        private readonly IDbAssignment _dbAssignment;
        private readonly AssignmentInputValidation _assignmentValidation;

        public AssignmentBusiness()
        {
            _dbAssignment = new DbAssignment();
            _assignmentValidation = new AssignmentInputValidation();
        }

        public List<Assignment> GetAllAssignments()
        {
            return _dbAssignment.GetAllAssignments();
        }
        public List<Assignment> GetAllActiveAssignments()
        {
            return _dbAssignment.GetAllActiveAssignments();
        }
        public List<Assignment> GetAllInactiveAssignments()
        {
            return _dbAssignment.GetAllInactiveAssignments();
        }
        public int CreateAssignment(Assignment assignment)
        {
            if (_assignmentValidation.CheckInput(assignment))
            {
                return _dbAssignment.CreateAssignment(assignment);
            }
            return -1;

        }

        public Assignment GetByAssignmentId(int id)
        {
            return _dbAssignment.GetByAssignmentId(id);
        }

        public int UpdateAssignment(Assignment assignment, int id)
        {
            //TODO validators 
            return _dbAssignment.UpdateAssignment(assignment, id);
        }

        public int MakeInactive(int id)
        {
            return _dbAssignment.MakeAssignmentInactive(id);
        }

        public int MakeAssignmentInactive(int id)
        {
            return _dbAssignment.MakeAssignmentInactive(id);
        }

        public List<string> GetAllAcademicLevels()
        {
            return _dbAssignment.GetAllAcademicLevels();
        }

        public List<string> GetAllSubjects()
        {
            return _dbAssignment.GetAllSubjects();
        }

        public List<Assignment> GetAllAssignmentsForUser(string userId)
        {
            return _dbAssignment.GetAllAssignmentsForUser(userId);
        }

        public List<Assignment> GetAllAssignmentsSolvedByUser(string userId)
        {
            return _dbAssignment.GetAllAssignmentsSolvedByUser(userId);
        }
    }
}
