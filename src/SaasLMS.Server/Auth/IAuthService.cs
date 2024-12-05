using System.Security.Claims;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Auth;

public interface IAuthService
{
    Task<ClaimsPrincipal> ValidateAccessTokenAsync(string accessToken);
    Task<string> GenerateAccessTokenAsync(User user);
}