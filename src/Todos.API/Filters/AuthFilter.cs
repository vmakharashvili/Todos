using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Todos.Core.Services.Abstract;

namespace Todos.API.Filters;

public class AuthFilter : IActionFilter
{
    private readonly IAuthService _authService;

    public AuthFilter(IAuthService authService)
    {
        _authService = authService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (user != null)
        {
            _authService.SetUser(Convert.ToInt32(user.Value));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}