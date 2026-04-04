using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Modules.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireMockAuthAttribute : TypeFilterAttribute
{
    public RequireMockAuthAttribute() : base(typeof(RequireMockAuthFilter))
    {
    }
}
