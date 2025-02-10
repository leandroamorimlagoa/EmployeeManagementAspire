using Domain.Core;
using Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class NotificationFilter : IActionFilter
{
    private readonly NotificationContext _notificationContext;
    private readonly AuthenticatedUser _authenticatedUser;
    private readonly ILogger<NotificationFilter> _logger;

    public NotificationFilter(NotificationContext notificationContext,
                                AuthenticatedUser authenticatedUser,
                                ILogger<NotificationFilter> logger)
    {
        this._notificationContext = notificationContext;
        this._authenticatedUser = authenticatedUser;
        this._logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (_notificationContext.HasNotifications)
        {
            var problemDetail = GetProblemDetails();

            _logger.LogInformation($"Validation errors occurred from user({_authenticatedUser.Id}): ({problemDetail.Detail})");

            context.Result = new ObjectResult(problemDetail) { StatusCode = problemDetail.Status };
        }
    }

    private ProblemDetails GetProblemDetails()
    {
        var messagesDetails = string.Join("|", _notificationContext.Notifications.Select(x => $"{x.DateTime.ToString("yyyy-MM-dd HH:mm:ss:fff")} - {x.Message}"));
        return new ProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = messagesDetails,
        };
    }
}
