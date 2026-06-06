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
                // ForceLogout clears both the JWT session and the auth cookie so the user
                // can re-authenticate cleanly (avoids the loop where Login redirects
                // authenticated-cookie users back to Home without a fresh JWT).
                context.Result = new RedirectToActionResult("ForceLogout", "Auth", null);
                context.ExceptionHandled = true;
            }
        }
    }
}
