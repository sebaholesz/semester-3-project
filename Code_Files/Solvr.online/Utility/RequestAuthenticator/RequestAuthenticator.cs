using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Utility.RequestAuthenticator
{
    /*this class and method is prepared for request authentication but does not need to be used now*/
    public static class RequestAuthenticator
    {
        public static void Authorize(string requestToken)
        {
            ///*this method should be used to authorize the request*/
            //RequestAuthenticator.Authorize(Request.Headers["request-token"].ToString());

            //var handler = new JwtSecurityTokenHandler();
            //var token = handler.ReadJwtToken(requestToken);
            //var claims = token.Claims;
        }
    }
}
