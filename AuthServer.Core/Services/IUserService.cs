using AuthServer.Core.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IUserService
    {
        Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUser);
        Task<Response<AppUserDto>> GetUserByNameAsync(string userName);

        Task<Response<AppUserDto>> UpdateUserPasswordByNameAsync(UpdateUserDto createUser);
        Task<Response<NoContent>> CreateUserRoles(string username, string role);


    }
}
