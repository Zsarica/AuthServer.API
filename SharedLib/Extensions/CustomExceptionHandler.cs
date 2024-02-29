using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLib.Dtos;
using SharedLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLib.Extensions
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app) 
        {
            app.UseExceptionHandler(conf =>
            {
                conf.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;

                        ErrorDto dto = null;
                        if (ex is  CustomException)
                        {
                            dto =  new ErrorDto(ex.Message,true);
                        }
                        else
                        {
                            dto = new ErrorDto(ex.Message, false);
                        }

                        var response = Response<NoDataDto>.Fail(dto, 500);

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
