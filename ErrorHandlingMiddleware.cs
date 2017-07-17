using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SwaggerExceptionHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await SetErrorResponseAsync(context, ex);
                throw;      // Re-throwing ensures exception is logged, however breaks response in Swagger UI.
            }
        }

        private Task SetErrorResponseAsync(HttpContext context, Exception exception)
        {
            var errorContent = JsonConvert.SerializeObject(new { myCustomErrorField = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(errorContent);
        }
    }
}
