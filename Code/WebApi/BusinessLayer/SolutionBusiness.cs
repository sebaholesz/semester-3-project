using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class SolutionBusiness
    {
        private DbSolutionIF _dbSolution = new DbSolution();

        public List<Solution> GetAllSolutions()
        {
            return _dbSolution.GetAllSolutions();
        }
        public int CreateSolution(Solution solution)
        {
            return _dbSolution.CreateSolution(solution);
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
    }
}
