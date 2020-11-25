using BusinessLayer.Validation;
using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class SolutionBusiness
    {
        private readonly DbSolutionIF dbSolution;
        private SolutionInputValidation validateSolution;

        public SolutionBusiness()
        {
            dbSolution = new DbSolution();
            validateSolution = new SolutionInputValidation();

        }

        public List<Solution> GetAllSolutions()
        {
            return dbSolution.GetAllSolutions();
        }

        public List<Solution> GetSolutionsTimestampOrderedByAssignmentId(int id)
        {
            return dbSolution.GetSolutionsTimestampOrderedByAssignmentId(id);
        }

        public int CreateSolution(Solution solution)
        {
            if (validateSolution.CheckInput(solution))
            {
                return dbSolution.CreateSolution(solution);
            }
            return -1;

        }
        public Solution GetBySolutionId(int id)
        {
            return dbSolution.GetBySolutionId(id);
        }

        public int UpdateSolution(Solution solution, int id)
        {
            return dbSolution.UpdateSolution(solution, id);
        }
        public int DeleteSolution(int id)
        {
            return dbSolution.DeleteSolution(id);
        }
    }
}
