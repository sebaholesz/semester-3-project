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
            try
            {
                return _dbAssignment.GetAllAssignments();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetActiveAssignmentsByPage(int pageNumber)
        {
            try
            {
                int start = (pageNumber - 1) * 12;
                return _dbAssignment.GetAssignmentsByPage(start);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllActiveAssignmentsNotPostedByUserPage(string userId, int pageNumber)
        {
            try
            {
                int start = (pageNumber - 1) * 12;
                return _dbAssignment.GetAllActiveAssignmentsNotPostedByUserPage(userId, start);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllAssignmentsForUserPage(string userId, int pageNumber)
        {
            try
            {
                int start = (pageNumber - 1) * 12;
                return _dbAssignment.GetAllAssignmentsForUserPage(userId, start);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetAssignmentsCount()
        {
            try
            {
                return _dbAssignment.GetAssignmentsCount();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetAssignmentsCountNotByUser(string userId)
        {
            try
            {
                return _dbAssignment.GetAssignmentsCountNotByUser(userId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetAssignmentsCountForUser(string userId)
        {
            try
            {
                return _dbAssignment.GetAssignmentsCountForUser(userId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object PaginationMetadata(int pageNumber)
        {
            try
            {
                int count = GetAssignmentsCount();
                int totalPages = (int)Math.Ceiling(count / 12.00);
                bool previousPage = pageNumber > 1 ? true : false;
                bool nextPage = pageNumber < totalPages ? true : false;
                var paginationMetadata = new
                {
                    count,
                    totalPages,
                    previousPage,
                    nextPage
                };
                return paginationMetadata;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId)
        {
            try
            {
                return _dbAssignment.GetAllActiveAssignmentsNotSolvedByUser(userId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
       
        //public List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId)
        //{
        //    try
        //    {
        //        return _dbAssignment.GetAllInactiveAssignmentsNotSolvedByUser(userId);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        
        public List<Assignment> GetAllActiveAssignments()
        {
            try
            {
                return _dbAssignment.GetAllActiveAssignments();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        //public List<Assignment> GetAllInactiveAssignments()
        //{
        //    try
        //    {
        //        return _dbAssignment.GetAllInactiveAssignments();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
        
        public int CreateAssignment(Assignment assignment)
        {
            try
            {
                if (_assignmentValidation.CheckInput(assignment))
                {                    
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
            try
            {
                return _dbAssignment.GetByAssignmentId(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object GetAssignmentCompleteData(int assignmentId)
        {
            try
            {
                Assignment assignment = _dbAssignment.GetByAssignmentId(assignmentId);
                if (!assignment.Equals(null))
                {
                    User user = _userBusiness.GetDisplayDataByUserId(assignment.UserId);

                    if (!user.Equals(null))
                    {
                        return new { Assignment = assignment, User = user };
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object GetAssignmentCompleteDataWithSolution(int assignmentId, string userId)
        {
            try
            {
                Assignment assignment = _dbAssignment.GetByAssignmentId(assignmentId);
                if (!assignment.Equals(null))
                {
                    User user = _userBusiness.GetDisplayDataByUserId(assignment.UserId);

                    if (!user.Equals(null))
                    {
                        Solution solution = SolutionBusiness.GetSolutionBusiness().GetSolutionForAssignmentByUser(assignmentId, userId);

                        if (!solution.Equals(null))
                        {
                            //TODO add conditional logic 
                            return new { Assignment = assignment, Solution = solution, User = user };
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

        public object GetAssignmentCompleteDataWithAcceptedSolution(int assignmentId)
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
                            //TODO add conditional logic 
                            return new { Assignment = assignment, Solution = solution, User = user };
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

        //public bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId)  
        //{
        //    try
        //    {
        //        return _dbAssignment.CheckIfUserAlreadySolvedThisAssignment(asignmentId, userId);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public bool CheckIfAssignmentIsStillActive(int assignmentId)
        //{
        //    try
        //    {
        //        return _dbAssignment.CheckIfAssignmentIsStillActive(assignmentId);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
       
        public int UpdateAssignment(Assignment assignment, int id)
        {
            try
            {
                //TODO validators 
                return _dbAssignment.UpdateAssignment(assignment, id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int MakeInactive(int id)
        {
            try
            {
                return _dbAssignment.MakeAssignmentInactive(id);
            }
            catch (Exception e)
            {
                throw e;
            }        
        }

        public List<Assignment> GetAllActiveAssignmentsNotPostedByUser(string userId)
        {
            try
            {
                return _dbAssignment.GetAllActiveAssignmentsNotPostedByUser(userId);
            }
            catch (Exception e)
            {

                throw e;
            }        
        }

        public int MakeAssignmentInactive(int id)
        {
            try
            {
                return _dbAssignment.MakeAssignmentInactive(id);
            }
            catch (Exception e)
            {
                throw;
            }        
        }

        public int MakeActive(int id)
        {
            try
            {
                return _dbAssignment.MakeAssignmentActive(id);
            }
            catch (Exception e)
            {
                throw e;
            }        
        }

        public List<string> GetAllAcademicLevels()
        {
            try
            {
                return _dbAssignment.GetAllAcademicLevels();

            }
            catch (Exception e)
            {
                throw e;
            }        
        }

        public List<string> GetAllSubjects()
        {
            try
            {
                return _dbAssignment.GetAllSubjects();
            }
            catch (Exception e)
            {

                throw e;
            }        
        }

        public List<Assignment> GetAllAssignmentsForUser(string userId)
        {
            try
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Assignment> GetAllAssignmentsSolvedByUser(string userId)
        {
            try
            {
                return _dbAssignment.GetAllAssignmentsSolvedByUser(userId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int CheckUserVsAssignment(int assignmentId, string userId)
        {
            try
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
                    if (!UserBusiness.GetUserBusiness().CheckIfUserExists(userId))
                    {
                        throw new Exception("User with this ID was not found");
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public byte[] GetFileFromDB(int assignmentId)
        {
            try
            {
                return _dbAssignment.GetFileFromDB(assignmentId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
