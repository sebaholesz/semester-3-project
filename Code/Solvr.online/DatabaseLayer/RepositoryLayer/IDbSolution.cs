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
        int GetSolutionsCountByAssignmentId(int assignmentId);
        Solution GetBySolutionId(int id);
        int UpdateSolution(Solution solution, int id);
        int DeleteSolution(int id);
        int ChooseSolution(int solutionId);
        List<string> GetAllSolversForAssignment(int assignmentId);
        Solution GetAcceptedSolutionForAssignment(int assignmentId);
        byte[] GetFileFromDB(int solutionId);
        Solution GetSolutionForAssignment(int assignmentId);
        Solution GetSolutionForAssignmentByUser(int assignmentId, string userId);
    }
}
