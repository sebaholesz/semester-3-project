using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbAssignment
    {
        int CreateAssignment(Assignment assignment);
        void GetFileFromDB(int id);
        List<Assignment> GetAllAssignments();
        List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllActiveAssignments();
        List<Assignment> GetAllInactiveAssignments();
        Assignment GetByAssignmentId(int id);
        bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId);
        int UpdateAssignment(Assignment assignment, int id);
        int MakeAssignmentInactive(int id);
        List<string> GetAllAcademicLevels();
        List<string> GetAllSubjects();
        int DeleteAssignment(int id);
        List<Assignment> GetAllAssignmentsForUser(string userId);
        List<Assignment> GetAllAssignmentsSolvedByUser(string userId);
        string GetAuthorUserId(int assignmentId);
    }
}
