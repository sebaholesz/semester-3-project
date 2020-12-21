using ModelLayer;
using System;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbSolution
    {
        int CreateSolution(Solution solution);
        List<Solution> GetAllSolutions();
        List<Solution> GetSolutionsByAssignmentId(int assignmentId);
        int GetSolutionsCountByAssignmentId(int assignmentId);
        Solution GetBySolutionId(int solutionId);
        int UpdateSolution(Solution solution, int solutionId);
        int DeleteSolution(int solutionId);
        int ChooseSolution(int solutionId);
        List<string> GetAllSolversForAssignment(int assignmentId);
        Solution GetAcceptedSolutionForAssignment(int assignmentId);
        byte[] GetFileFromDB(int solutionId);
        Solution GetSolutionForAssignment(int assignmentId);
        Solution GetSolutionForAssignmentByUser(int assignmentId, string userId);
        bool CheckIfUserIsSolutionAuthor(string userId, int solutionId);
    }
}
