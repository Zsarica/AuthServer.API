using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto login)
        {
            var result = await _authenticationService.CreateTokenAsync(login);

            return ActionResultInstance(result);  ///ActionresulInstance bizim oluşturduğumuz bir IAction Result dönen bir class. İçerisinde Response sınıfına 
                                                  //dönen status kodu verdiğimiz ObjectResult dönüyıor. ObjectResul da IAction result dan miras aldığı için kullanım uygun.
        }

        [HttpPost]
        public  IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var result =  _authenticationService.CreateTokenByClient(clientLoginDto);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto dto)
        {
            var result = await _authenticationService.RevokeRefreshTokenAsync(dto.RefreshToken);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)  //burada expirationdate kontrolü yapılmama ile ilgili bug var
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.RefreshToken);

            return ActionResultInstance(result);
        }



    }
}
