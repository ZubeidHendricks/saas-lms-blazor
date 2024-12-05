using Microsoft.AspNetCore.Authorization;
using SaasLMS.Server.Data;

namespace SaasLMS.Server.Auth;

public class TenantAuthorizationHandler : AuthorizationHandler<TenantRequirement>
{
    private readonly ITenantService _tenantService;

    public TenantAuthorizationHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantRequirement requirement)
    {
        var userTenantId = context.User.FindFirst(AuthConstants.Claims.TenantId)?.Value;

        if (userTenantId != null && 
            userTenantId == _tenantService.CurrentTenant.Id.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}