﻿using AuthServer.Core.Configure;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(AppUser user);

        ClientTokenDto CreateClientToken(ClientProp client);
    }
}
