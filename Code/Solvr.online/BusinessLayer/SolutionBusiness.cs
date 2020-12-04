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
        private readonly SolutionInputValidation _validateSolution;

        public SolutionBusiness()
        {
            _dbSolution = new DbSolution();
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

        public int ChooseSolution(int solutionId)
        {
            return _dbSolution.ChooseSolution(solutionId);
        }

        public Solution GetSolutionForUserByAssignmentId(string userId, int assignmentId)
        {
            return _dbSolution.GetSolutionForUserByAssignmentId(userId, assignmentId);
        }
    }
}
