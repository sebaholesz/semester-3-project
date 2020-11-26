using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface IDbAssignment
    {
        List<Assignment> GetAllAssignments();
        List<Assignment> GetAllActiveAssignments();
        List<Assignment> GetAllInactiveAssignments();
        int CreateAssignment(Assignment assignment);
        Assignment GetByAssignmentId(int id);
        int UpdateAssignment(Assignment assignment, int id);
        int MakeAssignmentInactive(int id);
        List<string> GetAllAcademicLevels();
        List<string> GetAllSubjects();
        int CreateAssignmentWithFile(Assignment assignment);
        void GetFileFromDB(int id);
    }
}
