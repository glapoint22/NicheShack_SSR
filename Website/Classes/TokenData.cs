using System;
using System.IdentityModel.Tokens.Jwt;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class TokenData : ITokenData<TokenData>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccessTokenExpiration { get; set; }

        public TokenData GetTokenData(JwtSecurityToken accessToken, RefreshToken refreshToken, Customer customer = null)
        {
            return new TokenData
            {
                AccessTokenExpiration = accessToken.ValidTo.ToString() + " UTC",
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = Uri.EscapeDataString(refreshToken.Id),
            };
        }
    }
}
