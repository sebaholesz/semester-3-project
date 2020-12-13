﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer;
using BusinessLayer;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthentication.Controllers
{
    [Route("apiV1/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] User loginUser)
        {
            try
            {
                IActionResult response = Unauthorized();

                User loginUserFromDB = UserBusiness.GetUserBusiness().GetUserByUserName(loginUser.UserName);
                loginUserFromDB.Password = loginUser.Password;

                if (UserBusiness.GetUserBusiness().AuthenticateUser(loginUserFromDB))
                {
                    var tokenString = GenerateJSONWebToken(loginUserFromDB);
                    response = Ok(new { token = tokenString });
                }
                return response;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("login-admin")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginAdmin([FromBody] User loginUser)
        {
            try
            {
                IActionResult response = Unauthorized();

                User loginUserFromDB = UserBusiness.GetUserBusiness().GetUserByUserName(loginUser.UserName);
                loginUserFromDB.Password = loginUser.Password;

                if (UserBusiness.GetUserBusiness().AuthenticateUser(loginUserFromDB) && UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(loginUserFromDB.UserName))
                {
                    var tokenString = GenerateJSONWebToken(loginUserFromDB);
                    response = Ok(new { token = tokenString });
                }
                return response;
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("login-internal")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginWithIdAndSecurityStamp([FromBody] User loginUser)
        {
            IActionResult response = Unauthorized();

            if (UserBusiness.GetUserBusiness().AuthenticateUserWithIdAndSecurityStamp(loginUser))
            {
                User user = UserBusiness.GetUserBusiness().GetUserById(loginUser.Id);
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                //new Claim("Role", UserBusiness.GetUserBusiness().GetUserRoleByUserName(userInfo.UserName)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}