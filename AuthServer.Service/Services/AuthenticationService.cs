using AuthServer.Core.Configure;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UOW;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<ClientProp> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _refteshTokenService;

        public AuthenticationService(IOptions<List<ClientProp>> clients, ITokenService tokenService, UserManager<AppUser> userManager, 
            IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> refteshTokenService) 
        {
            _clients = clients.Value;
            _tokenService = tokenService; 
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _refteshTokenService = refteshTokenService;
            
        }
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null) { 
                return Response<TokenDto>.Fail("Email or Password is wrong",400,true);
            }
            
            if(!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            var token = _tokenService.CreateToken(user);

            var refreshToken = await _refteshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            if(refreshToken == null)
            {
                await _refteshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, 
                    Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                refreshToken.Code = token.RefreshToken;
                refreshToken.Expiration = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200); 

        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.Id && x.Secret == clientLoginDto.Secret);
            if(client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId veya ClientSecret bulunamadı.", 404, true);
            }
            var clientToken = _tokenService.CreateClientToken(client);
            return Response<ClientTokenDto>.Success(clientToken, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var exstRefToken = await _refteshTokenService.Where(x=>x.Code == refreshToken).SingleOrDefaultAsync(); 

            if(exstRefToken == null)
            {
                return Response<TokenDto>.Fail("Refresh token not found.",404, true);
            }
            var user = await _userManager.FindByIdAsync(exstRefToken.UserId);

            if(user == null)
            {
                return Response<TokenDto>.Fail("User not found.", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(user);

            exstRefToken.Code = tokenDto.RefreshToken.ToString();
            exstRefToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(tokenDto, 200);
                    
        }

        public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var exstRefToken = await _refteshTokenService.Where(x=>x.Code == refreshToken).SingleOrDefaultAsync();

            if(exstRefToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found.", 404, true);
            }

            _refteshTokenService.Remove(exstRefToken);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(200);

        }
    }
}
