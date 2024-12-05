using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SaasLMS.Client.Auth;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration)
        : base(provider, navigation)
    {
        ConfigureHandler(new[] 
        { 
            configuration["API:BaseAddress"] 
        });
    }
}