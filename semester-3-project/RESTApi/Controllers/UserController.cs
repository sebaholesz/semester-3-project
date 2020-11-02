using BusinessLayer.Users;
using ModelLayer.User;
using System.Collections.Generic;
using System.Web.Http;

namespace RESTApi.Controllers
{
    public class UserController : ApiController
    {
        private UserBusiness _userBus;

        public UserController()
        {
            _userBus = new UserBusiness();
        }

        [HttpGet]
        public List<User> GetAllUsers()
        {
            return _userBus.GetAllUsers();
        }

        [HttpPost]
        public int InsertUser([FromBody] User user)
        {
            return _userBus.InsertUser(user);
        }
    }
}