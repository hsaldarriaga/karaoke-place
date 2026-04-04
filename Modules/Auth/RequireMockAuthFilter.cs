using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace karaoke_place.Modules.Auth;

public class RequireMockAuthFilter(CurrentUserContext currentUserContext) : IAsyncActionFilter
{
    private readonly CurrentUserContext _currentUserContext = currentUserContext;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var allowsAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<IAllowAnonymous>()
            .Any();

        if (allowsAnonymous)
        {
            await next();
            return;
        }

        var userId = _currentUserContext.GetUserId();
        if (userId == null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                status = "UNAUTHORIZED",
                error = "Missing or invalid bearer token."
            });
            return;
        }

        await next();
    }
}
