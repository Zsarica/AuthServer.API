﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dtos
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
