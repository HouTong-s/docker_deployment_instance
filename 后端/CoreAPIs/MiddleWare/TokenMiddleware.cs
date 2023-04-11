using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using System.IO;
using CoreAPIs.Common;
using JWT.Exceptions;

namespace CoreAPIs.MiddleWare
{
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using System.Net;

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
            /*
             * 基于token的拦截器中间件，除登录、验证码请求外都会检验token
             */
        {
            //Console.WriteLine(httpContext.Request.Path.Value); 
            if (!httpContext.Request.Path.Value.ToLower().Contains("login") && !httpContext.Request.Path.Value.ToLower().Contains("generatecaptcha"))
                //不是一个登录请求(请求路径不包括login)也不是验证码请求就要根据token进行拦截了
            {
                if (httpContext.Request.Headers.ContainsKey("jwt"))
                {
                    var token = httpContext.Request.Headers["jwt"];
                    try
                    {
                        TokenVerificationResult obj =  TokenManager.confirm(token);
                        int a = obj.id;
                        string b = obj.Role;
                        if (httpContext.Request.Path.Value.ToLower().Contains(b.ToLower()))
                        //请求路径里面包含了jwt码解析出来的角色，则放行。(因为我学生的控制器名称前面都有student，老师管理员同理)
                        {
                            httpContext.Request.Headers.Add("ids", Convert.ToString(a));
                            httpContext.Request.Headers.Add("role", Convert.ToString(b));
                        }
                        else
                        //否则说明角色错误，就进行拦截
                        {
                            return HandleExceptionAsync(httpContext, "错误的角色", 401);
                        }
                    }
                    catch (TokenExpiredException)
                    {
                        //表示过期
                        Console.WriteLine("expired");
                        return HandleExceptionAsync(httpContext, "登录已过期，请重新登录",401);
                    }
                    catch (SignatureVerificationException)
                    {
                        //表示验证不通过
                        Console.WriteLine("invalid");
                        return HandleExceptionAsync(httpContext, "错误的签名",401);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("error");
                        return HandleExceptionAsync(httpContext, "jwt码错误",401);
                    }
                }
                else
                {
                    Console.WriteLine("缺少jwt码，请登录");
                    return HandleExceptionAsync(httpContext, "缺少jwt码，请登录",401);
                }
            }
            return _next(httpContext);
        }
        private Task HandleExceptionAsync(HttpContext HttpContext, string msg,int code)
        {
            ResultModel error = new ResultModel
            {
                code = code,
                msg = msg
            };
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(error);
            HttpContext.Response.ContentType = "application/json;charset=utf-8";
            HttpContext.Response.StatusCode = 401;
            return HttpContext.Response.WriteAsync(result);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class Middleware1Extensions
    {
        public static IApplicationBuilder UseMiddleware1(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}
