using System.Security.Claims;
using karaoke_place.Modules.Users;
using Microsoft.AspNetCore.Http;

namespace karaoke_place.Api.Common;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor, UserService userService)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserService _userService = userService;

    public string? GetSubject()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return null;

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;
    }

    public string? GetEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return null;

        return user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("email")?.Value;
    }

    public Task<int?> GetUserIdAsync()
    {
        var subject = GetSubject();
        if (string.IsNullOrWhiteSpace(subject)) return Task.FromResult<int?>(null);

        return _userService.GetOrCreateByAuth0SubjectAsync(subject, GetEmail());
    }
}
