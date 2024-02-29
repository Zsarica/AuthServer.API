using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dtos
{
    //Refresh token dönmeyeceğimiz üyeelik gerektirmeyen autantication durumları için hazırlandı..
    public class ClientTokenDto   
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}
