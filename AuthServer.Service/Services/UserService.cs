using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SharedLib.Dtos;
using SharedLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUser)
        {
            var user = new AppUser { Email = createUser.Email , UserName = createUser.UserName};

            var result = await _userManager.CreateAsync(user,createUser.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x=>x.Description).ToList();

                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400);
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);
        }

        public async Task<Response<NoContent>> CreateUserRoles(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Response<NoContent>.Fail("Kullanıcı bulunamadı",StatusCodes.Status404NotFound,true);
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }
            await _userManager.AddToRoleAsync(user, role);

            return Response<NoContent>.Success(StatusCodes.Status201Created);


        }

        public async Task<Response<AppUserDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if(user == null)
            {
                return Response<AppUserDto>.Fail("Username not fount", 404, true);
            }
            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);
        }

        public async Task<Response<AppUserDto>> UpdateUserPasswordByNameAsync(UpdateUserDto updateUser)
        {
            var user = await _userManager.FindByNameAsync(updateUser.UserName);
            

            if (user == null)
            {
                return Response<AppUserDto>.Fail("Username not fount", 404, true);
            }
            var result = await _userManager.RemovePasswordAsync(user);
            if(result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, updateUser.Password);
            }

            if (!result.Succeeded)
            {
                return Response<AppUserDto>.Fail("Şifre değiştirilemedi",404,true);
            }

            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(user), 200);


        }
    }
}
