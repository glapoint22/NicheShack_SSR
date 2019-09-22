using System.IdentityModel.Tokens.Jwt;
using Website.Models;

namespace Website.Interfaces
{
    public interface ITokenData<T>
    {
        T GetTokenData(JwtSecurityToken accessToken, RefreshToken refreshToken, Customer customer = null);
    }
}
