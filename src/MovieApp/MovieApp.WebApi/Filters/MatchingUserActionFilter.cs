namespace MovieApp.WebApi.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieApp.Logic.Interfaces.Services;

/// <summary>
/// Action filter that enforces the route's user-id parameter matches the
/// currently authenticated user's id. Returns 401 if unauthenticated and 403
/// if authenticated as a different user.
/// </summary>
public sealed class MatchingUserActionFilter : IActionFilter
{
    private const int UnauthenticatedUserId = 0;

    private readonly ICurrentUserService _currentUserService;
    private readonly string _routeParameterName;

    public MatchingUserActionFilter(ICurrentUserService currentUserService, string routeParameterName)
    {
        this._currentUserService = currentUserService;
        this._routeParameterName = routeParameterName;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue(this._routeParameterName, out object? rawValue)
            || rawValue is not int requestedUserId)
        {
            context.Result = new BadRequestObjectResult(
                $"Missing or invalid route parameter '{this._routeParameterName}'.");
            return;
        }

        int currentUserId = this._currentUserService.UserId;
        if (currentUserId == UnauthenticatedUserId)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (currentUserId != requestedUserId)
        {
            context.Result = new ForbidResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
