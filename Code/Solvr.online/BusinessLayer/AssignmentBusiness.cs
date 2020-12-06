using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AssignmentBusiness
    {
        private readonly IDbAssignment _dbAssignment;
        private readonly SolutionBusiness _solutionBusiness;
        private readonly AssignmentInputValidation _assignmentValidation;

        public AssignmentBusiness()
        {
            _dbAssignment = new DbAssignment();
            _solutionBusiness = new SolutionBusiness();
            _assignmentValidation = new AssignmentInputValidation();
        }

        public List<Assignment> GetAllAssignments()
        {
            return _dbAssignment.GetAllAssignments();
        }

        public List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId)
        {
            return _dbAssignment.GetAllActiveAssignmentsNotSolvedByUser(userId);
        }
       
        public List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId)
        {
            return _dbAssignment.GetAllInactiveAssignmentsNotSolvedByUser(userId);
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

        public bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId)
        {
            return _dbAssignment.CheckIfUserAlreadySolvedThisAssignment(asignmentId, userId);
        }

        public bool CheckIfAssignmentIsStillActive(int assignmentId)
        {
            return _dbAssignment.CheckIfAssignmentIsStillActive(assignmentId);
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
            List<Assignment> assignments = _dbAssignment.GetAllAssignmentsForUser(userId);

            for (int i = 0; i < assignments.Count; i++)
            {
                if (!assignments[i].UserId.Equals(userId))
                {
                    throw new Exception("Cannot access assignments posted by other users");
                }
            }

            return assignments;
        }

        public List<Assignment> GetAllAssignmentsSolvedByUser(string userId)
        {
            return _dbAssignment.GetAllAssignmentsSolvedByUser(userId);
        }

        public int CheckUserVsAssignment(int assignmentId, string userId)
        {
            string authorUserId = _dbAssignment.GetAuthorUserId(assignmentId);
            List<string> allSolversForAssignment = _solutionBusiness.GetAllSolversForAssignment(assignmentId);

            if (authorUserId.Equals(userId))
            {
                return 1;
            }
            else if (allSolversForAssignment.Contains(userId))
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }
}
