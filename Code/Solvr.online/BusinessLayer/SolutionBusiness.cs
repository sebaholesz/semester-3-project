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

        public List<Solution> GetAllSolutions()
        {
            return _dbSolution.GetAllSolutions();
        }

        public List<Solution> GetSolutionsByAssignmentId(int id)
        {
            return _dbSolution.GetSolutionsByAssignmentId(id);
        }

        public int CreateSolution(Solution solution)
        {
            if (_validateSolution.CheckInput(solution))
            {
                return _dbSolution.CreateSolution(solution);
            }
            return -1;

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

        internal List<string> GetAllSolversForAssignment(int assignmentId)
        {
            return _dbSolution.GetAllSolversForAssignment(assignmentId);
        }
    }
}
