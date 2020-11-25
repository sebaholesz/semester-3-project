//using BusinessLayer;
//using ModelLayer;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace WebApi.Controllers
//{
//    public class APIUserController : ApiController
//    {
//        private UserBusiness _userBus;

//        public APIUserController()
//        {
//            _userBus = new UserBusiness();
//        }

//        [HttpGet]
//        public List<User> GetAllUsers()
//        {
//            return _userBus.GetAllUsers();
//        }

//        [HttpGet]
//        public User GetUserById(int id)
//        {
//            return _userBus.GetUserById(id);
//        }

//        [HttpPost]
//        public int InsertUser([FromBody] User user)
//        {
//            return _userBus.InsertUser(user);
//        }

//        [HttpPut]
//        public HttpResponseMessage UpdateUser([FromBody] User user, int id)
//        {
//            int noOfRows = _userBus.UpdateUser(user, id);
//            return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);

//        }

//        [HttpDelete]
//        public int DeleteUser(int id)
//        {
//            return _userBus.DeleteUser(id);
//        }
//    }
//}
