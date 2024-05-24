using Newtonsoft.Json;
using System.Net;

namespace Auction_API.GlobalExceptionHandling
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate _requestDelegate;
        public ExceptionHandling(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Customize the error response as needed
            var response = new
            {
                error = new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.Message // You can choose to include more details here
                }
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
