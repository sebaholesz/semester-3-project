using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public sealed class AssignmentBusiness
    {
        private static readonly AssignmentBusiness _assignmentBusinessInstance = new AssignmentBusiness();
        private readonly IDbAssignment _dbAssignment;
        private readonly AssignmentInputValidation _assignmentValidation;
        private readonly UserBusiness _userBusiness;

        private AssignmentBusiness()
        {
            _userBusiness = UserBusiness.GetUserBusiness();
            _dbAssignment = new DbAssignment();
            _assignmentValidation = new AssignmentInputValidation();
        }

        public static AssignmentBusiness GetAssignmentBusiness()
        {
            return _assignmentBusinessInstance;
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
            try
            {
                if (_assignmentValidation.CheckInput(assignment))
                {
                    
                    UserBusiness.GetUserBusiness().DecreaseUserCreadits(assignment.Price, assignment.UserId);
                    return _dbAssignment.CreateAssignment(assignment);
                }
                return -1;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public Assignment GetByAssignmentId(int id)
        {
            return _dbAssignment.GetByAssignmentId(id);
        }

        public object GetAssignmentCompleteData(int assignmentId)
        {

            Assignment assignment = _dbAssignment.GetByAssignmentId(assignmentId);
            if(!assignment.Equals(null))
            {
                User user = _userBusiness.GetDisplayDataByUserId(assignment.UserId);

                if(!user.Equals(null))
                {
                    return new { Assignment = assignment, User = user };
                }
            }
            return null;
        }

        public object GetAssignmentCompleteDataWithSolution(int assignmentId)
        {
            try
            {
                Assignment assignment = _dbAssignment.GetByAssignmentId(assignmentId);
                if (!assignment.Equals(null))
                {
                    User user = _userBusiness.GetDisplayDataByUserId(assignment.UserId);

                    if (!user.Equals(null))
                    {
                        Solution solution = SolutionBusiness.GetSolutionBusiness().GetAcceptedSolutionForAssignment(assignmentId);

                        if (!solution.Equals(null))
                        {
                            //TODO refactor this nasty code :D
                            if ((assignment.UserId.Equals(user.Id) && assignment.IsActive == false) || (solution.UserId.Equals(user.Id)))
                            {
                                return new { Assignment = assignment, Solution = solution, User = user };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
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

        public List<Assignment> GetAllActiveAssignmentsNotPostedByUser(string userId)
        {
            return _dbAssignment.GetAllActiveAssignmentsNotPostedByUser(userId);
        }

        public int MakeAssignmentInactive(int id)
        {
            return _dbAssignment.MakeAssignmentInactive(id);
        }

        public int MakeActive(int id)
        {
            return _dbAssignment.MakeAssignmentActive(id);
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
            List<string> allSolversForAssignment = SolutionBusiness.GetSolutionBusiness().GetAllSolversForAssignment(assignmentId);

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
                if(!UserBusiness.GetUserBusiness().CheckIfUserExists(userId))
                {
                    throw new Exception("User with this ID was not found");
                }
                return 0;
            }
        }
    }
}
