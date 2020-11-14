using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class SolutionBusiness
    {
        private readonly DbSolutionIF dbSolution;

        public SolutionBusiness()
        {
            dbSolution = new DbSolution();
        }

        public List<Solution> GetAllSolutions()
        {
            return dbSolution.GetAllSolutions();
        }
        public int CreateSolution(Solution solution)
        {
            return dbSolution.CreateSolution(solution);
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
