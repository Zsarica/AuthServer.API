using AuthServer.Core.Configure;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Configurations;
using SharedLib.SharedServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CustomTokenConfiguration _customToken;


        public TokenService(UserManager<AppUser> userManager,IOptions<CustomTokenConfiguration> customToken)
        {
            _userManager = userManager;
            _customToken = customToken.Value;
        }

        private string CreateRefreshToken()
        {
            var nByte = new Byte[32];

            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(nByte);

            return Convert.ToBase64String(nByte);
        }

        private async Task<IEnumerable<Claim>> GetClaims(AppUser appUser,List<string> audience)
        {
            var userRoles = await  _userManager.GetRolesAsync(appUser);
            string city = "";

            if(appUser.City != null)
            {
                city = appUser.City;
            }

            var userList  = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(JwtRegisteredClaimNames.Email,appUser.Email),
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("city",city),
                new Claim("birth-date",appUser.BirthDate.ToShortDateString())
            };

            userList.AddRange(audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            userList.AddRange(userRoles.Select(x=> new Claim(ClaimTypes.Role,x)));

            return userList;
        }
        private IEnumerable<Claim> GetClaimsByClient(ClientProp clientProp)
        {
            var claims = new List<Claim>();
            claims.AddRange(clientProp.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            var claimJti = new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            var claimSub = new Claim(JwtRegisteredClaimNames.Sub, clientProp.Id.ToString());
            claims.Add(claimJti);
            claims.Add(claimSub);
            
            return claims;
            
            
        }

        public ClientTokenDto CreateClientToken(ClientProp client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customToken.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customToken.SecurityKey);

            SigningCredentials signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customToken.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredential
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };

            return clientTokenDto;
        }

        public TokenDto CreateToken(AppUser user)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customToken.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_customToken.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customToken.SecurityKey);

            SigningCredentials signingCredential = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customToken.Issuer,
                expires:accessTokenExpiration,
                notBefore:DateTime.Now,
                claims:GetClaims(user,_customToken.Audience).Result,
                signingCredentials:signingCredential
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;

        }
    }
}
