using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Exceptions;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            //throw new CustomException("Custom hata test mesajıdır.");
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto)); 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name));
        }

        
        [HttpPost]
        public async Task<IActionResult> UpdateUserPasswordByName(UpdateUserDto updateUserDto)
        {
            return ActionResultInstance(await _userService.UpdateUserPasswordByNameAsync(updateUserDto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserRoles(string userName,string role)
        {
            return ActionResultInstance(await _userService.CreateUserRoles(userName, role));
        }

    }
}
