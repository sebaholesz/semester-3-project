using BusinessLayer.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [Route("Users")]
        [HttpGet]
        public List<object> GetAllUsers()
        {
            return _userBus.GetAllUsers();
        }
    }
}
