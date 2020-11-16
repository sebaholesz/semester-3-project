using ModelLayer;
using System.Collections.Generic;

namespace DatabaseLayer.RepositoryLayer
{
    public interface DbAcademicLevelIF
    {
        List<AcademicLevel> GetAllAcademicLevels();
    }
}
