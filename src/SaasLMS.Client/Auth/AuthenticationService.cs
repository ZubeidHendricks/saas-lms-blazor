using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal;
using System.Security.Claims;

namespace SaasLMS.Client.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IAccessTokenProvider tokenProvider, IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;
        _configuration = configuration;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            return token.Value;
        }

        throw new InvalidOperationException("Failed to get access token");
    }

    public async Task<ClaimsPrincipal> GetUserAsync()
    {
        var token = await GetAccessTokenAsync();
        // Parse JWT token and create ClaimsPrincipal
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new InvalidOperationException("Invalid token format");
        }

        var identity = new ClaimsIdentity(jsonToken.Claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }

    public bool IsInRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }
}