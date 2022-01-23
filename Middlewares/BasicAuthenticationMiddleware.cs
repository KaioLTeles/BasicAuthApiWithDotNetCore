
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoMfaSguApi.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public BasicAuthenticationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Encoding encoding = Encoding.GetEncoding("UTF-8");

            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string ecodeUsernameAndPassword = authHeader.Substring("Basic ".Length).Trim();                
                string usernameAndPassword = encoding.GetString(Convert.FromBase64String(ecodeUsernameAndPassword));
                int index = usernameAndPassword.IndexOf(":");
                var username = usernameAndPassword.Substring(0, index);
                var password = usernameAndPassword.Substring(index + 1);
                if (username.Equals(_config.GetValue<string>("BasicAuth:Username")) && password.Equals(_config.GetValue<string>("BasicAuth:Password")))
                {
                    await _next.Invoke(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    await httpContext.Response.WriteAsync("User or Password is incorrect!", encoding);
                    return;
                }

            }
            else
            {                
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsync("Please inform a valid Basic Credentials", encoding);
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
