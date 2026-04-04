using Microsoft.AspNetCore.Http;

namespace karaoke_place.Modules.Auth;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private static readonly Dictionary<string, int> MockTokenUserIds = new(StringComparer.Ordinal)
    {
        ["mock-user-1"] = 1,
        ["mock-user-2"] = 2,
        ["mock-host-1"] = 1
    };

    public int? GetUserId()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader)) return null;

        const string bearerPrefix = "Bearer ";
        if (!authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)) return null;

        var token = authHeader[bearerPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(token)) return null;

        if (MockTokenUserIds.TryGetValue(token, out var mappedUserId))
            return mappedUserId;

        if (token.StartsWith("mock-user-", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(token["mock-user-".Length..], out var parsedUserId)
            && parsedUserId > 0)
        {
            return parsedUserId;
        }

        return null;
    }
}
