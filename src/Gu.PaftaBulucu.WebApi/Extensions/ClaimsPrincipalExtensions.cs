using System.Security.Claims;

namespace Gu.PaftaBulucu.WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
#if DEBUG
            return "test@test.com";
#else
            return claimsPrincipal.FindFirst("principalId")?.Value;
#endif
        }
    }
}
