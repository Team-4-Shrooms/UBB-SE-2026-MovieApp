namespace MovieApp.Web.Filters
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public sealed class UnauthorizedApiFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is HttpRequestException { StatusCode: HttpStatusCode.Unauthorized })
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                context.ExceptionHandled = true;
            }
        }
    }
}
