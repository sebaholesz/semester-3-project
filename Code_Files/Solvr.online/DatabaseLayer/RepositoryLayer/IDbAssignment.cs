using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbAssignment
    {
        int CreateAssignment(Assignment assignment);
        byte[] GetFileFromDB(int assignmentId);
        List<Assignment> GetAllAssignments();
        int GetAssignmentsCount();
        int GetAssignmentsCountNotByUser(string userId);
        int GetAssignmentsCountForUser(string userId);
        List<Assignment> GetAssignmentsByPage(int start);
        List<Assignment> GetAllActiveAssignmentsNotPostedByUserPage(string userId, int start);
        List<Assignment> GetAllAssignmentsForUserPage(string userId, int start);
        List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllActiveAssignments();
        List<Assignment> GetAllInactiveAssignments();
        Assignment GetByAssignmentId(int assignmentId);
        bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId);
        bool CheckIfAssignmentIsStillActive(int assignmentId);
        int UpdateAssignment(Assignment assignment, int assignmentId);
        int MakeAssignmentInactive(int assignmentId);
        int MakeAssignmentActive(int assignmentId);
        List<string> GetAllAcademicLevels();
        List<string> GetAllSubjects();
        int DeleteAssignment(int assignmentId);
        List<Assignment> GetAllAssignmentsForUser(string userId);
        List<Assignment> GetAllAssignmentsSolvedByUser(string userId);
        string GetAuthorUserId(int assignmentId);
        List<Assignment> GetAllActiveAssignmentsNotPostedByUser(string userId);
        bool CheckIfHasAcceptedSolution(int assignmentId);
    }
}
