using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("apiV1/")]
    public class APIUserController : ControllerBase
    {
        [Route("user/add-credit/{id}")]
        [HttpPut]
        public IActionResult AddCredits([FromBody] User user, string id)
        {
            try
            {
                int credit = (int)user.Credit;
                int noOfRowsAffected = UserBusiness.GetUserBusiness().IncreaseUserCredits(credit, id);
                if (noOfRowsAffected > 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("user/remove-credit/{id}")]
        [HttpPut]
        public IActionResult RemoveCredits([FromBody] User user, string id)
        {
            try
            {
                int credit = (int)user.Credit;
                int noOfRowsAffected = UserBusiness.GetUserBusiness().DecreaseUserCredits(credit, id);
                if (noOfRowsAffected > 0)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("user/get-credit/{id}")]
        [HttpGet]
        public IActionResult GetUserCredit(string id)
        {
            try
            {
                int credit = UserBusiness.GetUserBusiness().GetUserCredits(id);

                if (credit >= 0)
                {
                    return Ok(credit);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("user")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<User> users = UserBusiness.GetUserBusiness().GetAllUsers();

                if (users.Count() > 0)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("user/get-concurrency-stamp/{id}")]
        [HttpGet]
        public IActionResult GetUserConcurrencyStamp(string id)
        {
            try
            {
                string stamp = UserBusiness.GetUserBusiness().GetUserConcurrencyStamp(id);

                if (stamp.Length >= 0)
                {
                    return Ok(stamp);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //[HttpGet]
        //public List<User> GetAllUsers()
        //{
        //    return _userBus.GetAllUsers();
        //}

        //[HttpGet]
        //public User GetUserById(int id)
        //{
        //    return _userBus.GetUserById(id);
        //}

        //[HttpPost]
        //public int InsertUser([FromBody] User user)
        //{
        //    return _userBus.InsertUser(user);
        //}

        //[HttpPut]
        //public HttpResponseMessage UpdateUser([FromBody] User user, int id)
        //{
        //    int noOfRows = _userBus.UpdateUser(user, id);
        //    return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);

        //}

        //[HttpDelete]
        //public int DeleteUser(int id)
        //{
        //    return _userBus.DeleteUser(id);
        //}
    }
}
