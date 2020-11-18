using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface DbSolutionIF
    {
        List<Solution> GetAllSolutions();
        int CreateSolution(Solution solution);
        Solution GetBySolutionId(int id);
        int UpdateSolution(Solution solution, int id);
        int DeleteSolution(int id);
        List<Solution> GetSolutionsByAssignmentId(int id);
    }
}
