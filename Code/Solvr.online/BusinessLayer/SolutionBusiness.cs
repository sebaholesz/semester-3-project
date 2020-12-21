using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public sealed class SolutionBusiness
    {
        private static readonly SolutionBusiness _solutionBusinessInstance = new SolutionBusiness();
        private readonly IDbSolution _dbSolution;
        private readonly SolutionInputValidation _validateSolution;

        private SolutionBusiness()
        {
            _dbSolution = new DbSolution();
            _validateSolution = new SolutionInputValidation();
        }

        public static SolutionBusiness GetSolutionBusiness()
        {
            return _solutionBusinessInstance;
        }

        public int CreateSolution(Solution solution)
        {
            try
            {
                if (_validateSolution.CheckInput(solution))
                {
                    int result = _dbSolution.CreateSolution(solution);
                    if (result > 0)
                    {
                        int queueLengthAfter = GetSolutionsCountByAssignmentId(solution.AssignmentId);
                        return queueLengthAfter;
                    }
                }
                return -1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Solution> GetAllSolutions()
        {
            try
            {
                return _dbSolution.GetAllSolutions();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Solution> GetSolutionsByAssignmentId(int assignmentId)
        {
            try
            {
                return _dbSolution.GetSolutionsByAssignmentId(assignmentId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }   

        public int GetSolutionsCountByAssignmentId(int assignmentId)
        {
            try
            {
                return _dbSolution.GetSolutionsCountByAssignmentId(assignmentId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Solution GetBySolutionId(int solutionId)
        {
            try
            {
                return _dbSolution.GetBySolutionId(solutionId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateSolution(Solution solution, int solutionId)
        {
            try
            {
                return _dbSolution.UpdateSolution(solution, solutionId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public int DeleteSolution(int solutionId)
        {
            try
            {
                return _dbSolution.DeleteSolution(solutionId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ChooseSolution(int solutionId, int assignmentId)
        {
            try
            {
                bool successfulyAccepted = _dbSolution.ChooseSolution(solutionId) == 1;
                if (successfulyAccepted)
                {
                    bool successfulyMadeInactive = AssignmentBusiness.GetAssignmentBusiness().MakeInactive(assignmentId) == 1;
                    Solution solution = GetBySolutionId(solutionId);
                    
                    Assignment assignment = AssignmentBusiness.GetAssignmentBusiness().GetByAssignmentId(assignmentId);
                    bool successfulyAdded = UserBusiness.GetUserBusiness().IncreaseUserCredits(assignment.Price, solution.UserId) == 1;
                    return successfulyAccepted && successfulyMadeInactive && successfulyAdded;
                    //return successfulyAccepted;
                }
                return false;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public List<string> GetAllSolversForAssignment(int assignmentId)
        {
            try
            {
                return _dbSolution.GetAllSolversForAssignment(assignmentId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Solution GetAcceptedSolutionForAssignment(int assignmentId)
        {
            try
            {
                Solution solution = _dbSolution.GetAcceptedSolutionForAssignment(assignmentId);
                return solution ?? null;
            }
            catch (Exception e)
            {   
                throw e;
            }
        }

        public byte[] GetFileFromDB(int solutionId)
        {
            try
            {
                return _dbSolution.GetFileFromDB(solutionId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckIfUserCanDownloadSolutionFile(int solutionId, string userId)
        {
            Solution solution = GetBySolutionId(solutionId);
            return (userId == solution.UserId || (AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(solution.AssignmentId, userId) == 1 && solution.Accepted));
        }

        public Solution GetSolutionForAssignment(int assignmentId)
        {
            try
            {
                Solution solution = _dbSolution.GetSolutionForAssignment(assignmentId);
                return solution ?? null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Solution GetSolutionForAssignmentByUser(int assignmentId, string userId)
        {
            try
            {
                Solution solution = _dbSolution.GetSolutionForAssignmentByUser(assignmentId, userId);
                return solution ?? null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckIfUserIsSolutionAuthor(string userId, int solutionId)
        {
            try
            {
                return _dbSolution.CheckIfUserIsSolutionAuthor(userId, solutionId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
