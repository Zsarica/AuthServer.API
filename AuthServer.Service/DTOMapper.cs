using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    internal class DTOMapper:Profile
    {
        public DTOMapper()
        {
            CreateMap<ProductDto,Product>().ReverseMap();
            CreateMap<AppUserDto, AppUser>().ReverseMap();
        }
    }
}
