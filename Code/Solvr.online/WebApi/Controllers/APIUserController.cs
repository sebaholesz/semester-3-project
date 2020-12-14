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
        /*ONLY THE USER HIMSELF*/
        [Route("user/add-credit")]
        [HttpPut]
        public IActionResult AddCredits([FromBody] int value)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                int noOfRowsAffected = UserBusiness.GetUserBusiness().IncreaseUserCredits(value, userId);
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

        /*ONLY THE ADMIN*/
        [Route("user-admin/add-credit/{id}")]
        [HttpPut]
        public IActionResult AdminAddCredits([FromBody] int value, string id)
        {
            try
            {
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);
                
                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    int noOfRowsAffected = UserBusiness.GetUserBusiness().IncreaseUserCredits(value, id);
                    if (noOfRowsAffected > 0)
                    {
                        return Ok();
                    }
                }
                return NotFound();
                
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        
        [Route("user-admin/remove-credit/{id}")]
        [HttpPut]
        public IActionResult RemoveCredits([FromBody] int value, string id)
        {
            try
            {
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);

                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    int noOfRowsAffected = UserBusiness.GetUserBusiness().DecreaseUserCredits(value, id);
                    if (noOfRowsAffected > 0)
                    {
                        return Ok();
                    }
                }
                return NotFound();
                
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY THE USER HIMSELF*/
        [Route("user/get-credit")]
        [HttpGet]
        public IActionResult GetUserCredit()
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                int credit = UserBusiness.GetUserBusiness().GetUserCredits(userId);

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
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);

                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    List<User> users = UserBusiness.GetUserBusiness().GetAllUsers();
                    if (users.Count() > 0)
                    {
                        return Ok(users);
                    }
                }
                    return NotFound();
                
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
