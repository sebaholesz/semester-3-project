using DataAccessLayer;
using DataAccessLayer.RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Users
{
    public class UserBusiness
    {
        public List<object> GetAllUsers()
        {
            DbUserIF dbUser = new DbUser();
            return dbUser.GetAllUsers();
        }
    }
}
