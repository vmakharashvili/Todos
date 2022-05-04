using System.Net;
using System.Text.Json;
using Todos.Core.Exceptions;

namespace Todos.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(ex, context, logger);
        }
    }

    private static async Task HandleException(Exception exception, HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
    {
        var code = HttpStatusCode.InternalServerError;

        if (exception is DomainException)
        {
            code = HttpStatusCode.BadRequest;
        }

        if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
        }

        var result = new
        {
            Error = exception.Message
        };

        if (code != HttpStatusCode.BadRequest)
        {
            logger.LogError(exception, "Error");
        }

        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsJsonAsync(result);
    }
}