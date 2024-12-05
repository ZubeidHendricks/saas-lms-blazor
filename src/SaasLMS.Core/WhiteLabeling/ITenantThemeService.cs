namespace SaasLMS.Core.WhiteLabeling;

public interface ITenantThemeService
{
    Task<TenantTheme> GetThemeAsync(Guid tenantId);
    Task<TenantTheme> UpdateThemeAsync(TenantTheme theme);
    Task<bool> UpdateLogoAsync(Guid tenantId, Stream logoStream, string contentType);
    Task<bool> UpdateFaviconAsync(Guid tenantId, Stream faviconStream, string contentType);
    Task<string> GenerateThemeCssAsync(Guid tenantId);
    Task<Dictionary<string, string>> GetEmailTemplatesAsync(Guid tenantId);
    Task UpdateEmailTemplateAsync(Guid tenantId, string templateName, string content);
    Task<bool> ValidateCustomDomainAsync(string domain);
    Task<bool> ConfigureCustomDomainAsync(Guid tenantId, string domain);
}