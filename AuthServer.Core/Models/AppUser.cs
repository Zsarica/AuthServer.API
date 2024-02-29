using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class AppUser:IdentityUser
    {
        public string? City { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
