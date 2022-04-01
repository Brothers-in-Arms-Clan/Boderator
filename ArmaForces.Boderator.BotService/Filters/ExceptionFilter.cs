using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArmaForces.Boderator.BotService.Filters;

public class ExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        OnException(context);
        return Task.CompletedTask;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ArgumentNullException) HandleValidationError(context);
        if (context.Exception is NotImplementedException) HandleNotImplemented(context);
    }

    private static void HandleNotImplemented(ExceptionContext context)
    {
        context.Result = new NotImplementedResult();
        context.ExceptionHandled = true;
    }

    private static void HandleValidationError(ExceptionContext context)
    {
        var error = new
        {
            Message = "Validation error",
            Details = context.Exception.Message
        };

        context.Result = new BadRequestObjectResult(error);
        context.ExceptionHandled = true;
    }
}
