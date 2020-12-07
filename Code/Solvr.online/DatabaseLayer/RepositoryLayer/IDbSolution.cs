using ModelLayer;
using System;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbSolution
    {
        int CreateSolution(Solution solution);
        List<Solution> GetAllSolutions();
        List<Solution> GetSolutionsByAssignmentId(int id);
        Solution GetBySolutionId(int id);
        Solution GetSolutionByAssignmentId(int assignmentId);
        int UpdateSolution(Solution solution, int id);
        int DeleteSolution(int id);
        int ChooseSolution(int solutionId);
        List<string> GetAllSolversForAssignment(int assignmentId);
    }
}
