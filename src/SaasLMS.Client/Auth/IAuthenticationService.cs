using System.Security.Claims;

namespace SaasLMS.Client.Auth;

public interface IAuthenticationService
{
    Task<string> GetAccessTokenAsync();
    Task<ClaimsPrincipal> GetUserAsync();
    bool IsInRole(ClaimsPrincipal user, string role);
}