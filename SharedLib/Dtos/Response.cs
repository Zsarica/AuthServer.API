using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLib.Dtos
{
    public class Response<T> where  T:class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private  set; }
        [JsonIgnore]
        public bool IsSuccess { get; private set; }
        public ErrorDto Error { get; private set; }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccess = true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccess = true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T> { StatusCode = statusCode, Error = errorDto, IsSuccess = false };
        }

        public static Response<T> Fail(string errorMessage, int statusCode,bool isShow)
        {
            var errosDto = new ErrorDto(errorMessage, isShow);

            return new Response<T> {  Error = errosDto, StatusCode = statusCode, IsSuccess = false };
        }



    }
}
