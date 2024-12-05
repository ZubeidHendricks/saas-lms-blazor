using SaasLMS.Server.Data;

namespace SaasLMS.Server.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var host = context.Request.Host.Value;

        // Extract subdomain from host
        string identifier = host.Split('.')[0];

        try
        {
            var tenant = await tenantService.GetTenantAsync(identifier);
            tenantService.SetCurrentTenant(tenant);
        }
        catch (Exception ex)
        {
            // Log the error
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid tenant" });
            return;
        }

        await _next(context);
    }
}