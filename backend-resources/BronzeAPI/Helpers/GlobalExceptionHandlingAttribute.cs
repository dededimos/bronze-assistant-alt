using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BronzeAPI.Helpers
{
    /// <summary>
    /// Adds Global Exception handling when registered as a filter to the APIControllers ,
    /// Any Unhandled exceptions are
    /// </summary>
    public class GlobalExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                var details = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = context.Exception.Message,
                    Detail = context.Exception.StackTrace
                };
                context.Result = new ObjectResult(details)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
