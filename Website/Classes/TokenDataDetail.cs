using System.IdentityModel.Tokens.Jwt;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class TokenDataDetail : TokenData, ITokenData<TokenDataDetail>
    {
        public CustomerDTO Customer { get; set; }

        public new TokenDataDetail GetTokenData(JwtSecurityToken accessToken, RefreshToken refreshToken, Customer customer)
        {
            TokenData tokenData = GetTokenData(accessToken, refreshToken);
            return new TokenDataDetail
            {
                AccessToken = tokenData.AccessToken,
                RefreshToken = tokenData.RefreshToken,
                AccessTokenExpiration = tokenData.AccessTokenExpiration,
                Customer = new CustomerDTO
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email
                }
            };
        }
    }
}
