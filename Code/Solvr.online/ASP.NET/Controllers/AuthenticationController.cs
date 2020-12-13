using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ASP.NET.Controllers
{
    public static class AuthenticationController
    {
        public static string CreateJWT(UserManager<User> _userManager, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string urlCreateJWT = "https://localhost:44316/apiV1/login";
                HttpResponseMessage createJWTRM = client.PostAsync(urlCreateJWT, new StringContent(JsonConvert.SerializeObject(new { username, password }), Encoding.UTF8, "application/json")).Result;

                if (createJWTRM.IsSuccessStatusCode)
                {
                    dynamic tokenObject = createJWTRM.Content.ReadAsAsync<object>().Result;
                    string token = tokenObject.token;

                    if (!token.Equals("") && token.Length > 0)
                    {
                        User user = _userManager.FindByNameAsync(username).Result;
                        IdentityResult ir = _userManager.SetAuthenticationTokenAsync(user, "SolvrOnline", "JWT", token).Result;
                        if (ir == IdentityResult.Success)
                        {
                            return token;
                        }
                    }
                }
                return null;
            }
        }

        public static string CreateJWTwithIdAndSecurityStamp(UserManager<User> _userManager, SignInManager<User> _signInManager, string id, string securityStamp)
        {
            using (HttpClient client = new HttpClient())
            {
                string urlCreateJWTwithSecurityStamp = "https://localhost:44316/apiV1/login-internal";
                HttpResponseMessage createJWTwithSecurityStampRM = client.PostAsync(urlCreateJWTwithSecurityStamp, new StringContent(JsonConvert.SerializeObject(new { id, securityStamp }), Encoding.UTF8, "application/json")).Result;

                if (createJWTwithSecurityStampRM.IsSuccessStatusCode)
                {
                    dynamic tokenObject = createJWTwithSecurityStampRM.Content.ReadAsAsync<object>().Result;
                    string token = tokenObject.token;

                    if (!token.Equals("") && token.Length > 0)
                    {
                        User user = _userManager.FindByIdAsync(id).Result;
                        IdentityResult ir = _userManager.SetAuthenticationTokenAsync(user, "SolvrOnline", "JWT", token).Result;
                        if (ir == IdentityResult.Success)
                        {
                            return token;
                        }
                    }
                }
                return null;
            }
        }

        public static async Task<AuthenticationHeaderValue> GetAuthorizationHeaderAsync(UserManager<User> _userManager, SignInManager<User> _signInManager, User user)
        {
            try  
            {
                using (HttpClient client = new HttpClient())
                {
                    string JWTtoken = _userManager.GetAuthenticationTokenAsync(user, "SolvrOnline", "JWT").Result;
                    if(JWTtoken == null)
                    {
                        JWTtoken = CreateJWTwithIdAndSecurityStamp(_userManager, _signInManager, user.Id, user.SecurityStamp);
                        if (JWTtoken != null)
                        {
                            await _userManager.UpdateSecurityStampAsync(user);
                        }
                        else
                        {
                            throw new UnauthorizedAccessException("Could not authenticate user");
                        }
                    }
                    return new AuthenticationHeaderValue("Bearer", JWTtoken); 
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string GenerateRequestToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("testtwojimamy123"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Website, "www.solver.online"),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim("AuthenticationString", "ifTHISstringWITHallOFourNAMESwhichAREmarosMARTINsebiSAMandSEBAisPRESENT...theREQUESTshouldBEaccepted"),
                new Claim("Now", DateTime.UtcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken("SolvrOnline",
                    "SolvrOnline",
                    claims,
                    expires: DateTime.Now.AddMinutes(3),
                    signingCredentials: credentials);

                JwtSecurityTokenHandler jwtth = new JwtSecurityTokenHandler();
                string tokenF = jwtth.WriteToken(token);
                return tokenF;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
