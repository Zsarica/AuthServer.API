using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLib.Configurations;
using SharedLib.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Extensions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenConfiguration tokenConfiguration)
        {
            
            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
               
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenConfiguration.Issuer,
                    ValidAudience = tokenConfiguration.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenConfiguration.SecurityKey),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
