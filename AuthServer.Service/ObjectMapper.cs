using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        private static Lazy<IMapper> lazy = new Lazy<IMapper>(() => 
        { 
            var config =  new MapperConfiguration(cnf => 
            {
                cnf.AddProfile<DTOMapper>();
            });
            return config.CreateMapper();
        });

        public static IMapper Mapper { get { return lazy.Value; } }
    }
}
