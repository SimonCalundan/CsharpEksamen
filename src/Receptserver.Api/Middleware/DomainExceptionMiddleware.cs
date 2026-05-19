using Receptserver.Core.Dtos;
using Receptserver.Core.Exceptions;

namespace Receptserver.Api.Middleware;

// Catches business-layer exceptions and maps them to HTTP responses.
// Keeps controllers free of try/catch boilerplate.
public class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DomainExceptionMiddleware> _logger;

    public DomainExceptionMiddleware(RequestDelegate next, ILogger<DomainExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message, ex.Field));
        }
        catch (UdleveringNotAllowedException ex)
        {
            _logger.LogWarning(ex, "Udlevering not allowed");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Unhandled domain exception");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
    }
}
