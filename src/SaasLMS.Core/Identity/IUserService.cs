namespace SaasLMS.Core.Identity;

public interface IUserService
{
    Task<ApplicationUser> GetUserByIdAsync(string userId);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user, string password);
    Task UpdateUserAsync(ApplicationUser user);
    Task<bool> IsEmailAvailableAsync(string email, Guid tenantId);
    Task<List<ApplicationUser>> GetUsersByTenantAsync(Guid tenantId, UserType? type = null);
    Task<bool> DeleteUserAsync(string userId);
}