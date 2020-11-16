using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface DbSubjectIF
    {
        List<Subject> GetAllSubjects();
    }
}
