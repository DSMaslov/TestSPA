using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSPA.Middleware
{
    public class ExceptionMiddleware
    {
        readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json; charset=UTF-8";
                context.Response.StatusCode = 500;

                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { ex.Message }));

                context.Response.Body.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
