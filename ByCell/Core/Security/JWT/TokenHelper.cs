using Core.Entities.Concrete;
using Core.Extensions.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.JWT
{
    public class TokenHelper : ITokenHelper
    {

        public IConfiguration Configuration { get; }
        private TokenConfig _tokenConfig;
        private DateTime _accessTokenExpiration;
        public TokenHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenConfig = Configuration.GetSection("TokenConfig").Get<TokenConfig>();

        }
        public AccessToken CreateToken(User user)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenConfig.AccessTokenExpiration);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = CreateJwtSecurityToken(_tokenConfig, user, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration,
                RefreshToken= CreateRefreshToken()
            };

        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenConfig tokenConfig, User user,
            SigningCredentials signingCredentials)
        {
            var securityToken = new JwtSecurityToken(
                issuer: tokenConfig.Issuer,
                audience: tokenConfig.Audience,
                expires: _accessTokenExpiration,
                claims: SetClaims(user),
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials
            );
            return securityToken;
        }
        public string CreateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private IEnumerable<Claim> SetClaims(User user)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(user.Id.ToString());
            claims.AddEmail(user.Email);
            claims.AddName($"{user.FirstName} {user.LastName}");

            return claims;
        }
    }
}
