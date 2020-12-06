using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class SolutionBusiness
    {
        private readonly IDbSolution _dbSolution;
        private readonly AssignmentBusiness _assignmentBusiness;
        private readonly SolutionInputValidation _validateSolution;

        public SolutionBusiness()
        {
            _dbSolution = new DbSolution();
            _assignmentBusiness = new AssignmentBusiness();
            _validateSolution = new SolutionInputValidation();

        }

        public int CreateSolution(Solution solution)
        {
            if (_validateSolution.CheckInput(solution))
            {
                List<Solution> solutionsBefore = _dbSolution.GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthBefore = solutionsBefore.Count;

                //check if the last one that is in the queue was earlier than the current one
                if (queueLengthBefore > 0)
                {
                    if (DateTime.Compare(solution.Timestamp, solutionsBefore[queueLengthBefore - 1].Timestamp) <= 0)
                    {
                        return -1;
                    }
                }

                //check that the assignment is still active
                if(!_assignmentBusiness.CheckIfAssignmentIsStillActive(solution.AssignmentId))
                {
                    throw new Exception("Cannot post solutions to inactive assignments");
                }

                List<Solution> solutionsAfter = _dbSolution.GetSolutionsByAssignmentId(solution.AssignmentId);
                int queueLengthAfter = solutionsAfter.Count;

                if (_dbSolution.CreateSolution(solution) > 0)
                {
                    return queueLengthAfter + 1;
                }
            }
            return -1;
        }

        public List<Solution> GetAllSolutions()
        {
            return _dbSolution.GetAllSolutions();
        }

        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            return _dbSolution.GetSolutionsByAssignmentId(id);
        }

        public Solution GetBySolutionId(int id)
        {
            return _dbSolution.GetBySolutionId(id);
        }

        public int UpdateSolution(Solution solution, int id)
        {
            return _dbSolution.UpdateSolution(solution, id);
        }
        
        public int DeleteSolution(int id)
        {
            return _dbSolution.DeleteSolution(id);
        }

        public bool ChooseSolution(int solutionId, int assignmentId)
        {
            bool successfulyAccepted = _dbSolution.ChooseSolution(solutionId) == 1 ? true : false; ;
            bool successfulyMadeInactive = _assignmentBusiness.MakeAssignmentInactive(assignmentId) == 1 ? true : false;
            return successfulyAccepted && successfulyMadeInactive;
        }

        public Solution GetSolutionByAssignmentId(int assignmentId)
        {
            Solution solution = _dbSolution.GetSolutionByAssignmentId(assignmentId);

            if (!solution.Equals(null))
            {
                return solution;
            }
            else
            {
                throw new Exception("Could not find your solution");
            }
        }

        public List<string> GetAllSolversForAssignment(int assignmentId)
        {
            return _dbSolution.GetAllSolversForAssignment(assignmentId);
        }
    }
}
