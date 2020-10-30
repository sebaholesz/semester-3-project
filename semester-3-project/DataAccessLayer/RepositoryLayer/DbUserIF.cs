using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryLayer
{
    public interface DbUserIF
    {
        List<object> GetAllUsers();
    }
}
