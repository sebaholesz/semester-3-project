using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class SubjectBusiness
    {
        private readonly DbSubjectIF dbSubject;

        public SubjectBusiness()
        {
            dbSubject = new DbSsubject();
        }

        public List<Subject> GetAllSubjects()
        {
            return dbSubject.GetAllSubjects();
        }
    }
}
