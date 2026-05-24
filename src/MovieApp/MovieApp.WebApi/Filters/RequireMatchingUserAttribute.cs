namespace MovieApp.WebApi.Filters;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Marks an endpoint as requiring the route's user-id parameter to match the
/// currently authenticated user. The route parameter defaults to <c>userId</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequireMatchingUserAttribute : TypeFilterAttribute
{
    public RequireMatchingUserAttribute(string routeParameterName = "userId")
        : base(typeof(MatchingUserActionFilter))
    {
        this.Arguments = new object[] { routeParameterName };
    }
}
