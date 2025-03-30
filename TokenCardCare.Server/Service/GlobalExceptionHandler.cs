using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TokenCardCare.Server.Model;

namespace TokenCardCare.Server.Service;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var code = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        httpContext.Response.StatusCode = (int)code;
        await httpContext.Response.WriteAsJsonAsync(new ApiResponse 
        { 
            Code = httpContext.Response.StatusCode, 
            Message = exception.Message 
        }, cancellationToken: cancellationToken);

        return true;
    }
}
