using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbAssignment
    {
        List<Assignment> GetAllAssignments();
        List<Assignment> GetAllActiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllInactiveAssignmentsNotSolvedByUser(string userId);
        List<Assignment> GetAllActiveAssignments();
        List<Assignment> GetAllInactiveAssignments();
        int CreateAssignment(Assignment assignment);
        Assignment GetByAssignmentId(int id);
        bool CheckIfUserAlreadySolvedThisAssignment(int asignmentId, string userId)
        int UpdateAssignment(Assignment assignment, int id);
        int DeleteAssignment(int id);
        int MakeAssignmentInactive(int id);
        List<string> GetAllAcademicLevels();
        List<string> GetAllSubjects();
        void GetFileFromDB(int id);
        List<Assignment> GetAllAssignmentsForUser(string userId);
        List<Assignment> GetAllAssignmentsSolvedByUser(string userId);
    }
}
