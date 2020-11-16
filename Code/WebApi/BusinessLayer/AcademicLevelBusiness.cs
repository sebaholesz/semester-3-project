using DatabaseLayer.DataAccessLayer;
using DatabaseLayer.RepositoryLayer;
using ModelLayer;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class AcademicLevelBusiness
    {
        private readonly DbAcademicLevelIF dbal;

        public AcademicLevelBusiness()
        {
            dbal = new DbAcademicLevel();
        }

        public List<AcademicLevel> GetAllAcademicLevels()
        {
            List<AcademicLevel> levels = dbal.GetAllAcademicLevels();
            return levels;
        }
    }
}
