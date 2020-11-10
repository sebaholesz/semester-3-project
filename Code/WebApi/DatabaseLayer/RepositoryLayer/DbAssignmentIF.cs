using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLayer.RepositoryLayer
{
    public interface DbAssignmentIF
    {
        List<Assignment> GetAllAssignments();
        Boolean CreateAssignment(Assignment assignment);
    }
}
