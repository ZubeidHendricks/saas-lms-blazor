namespace SaasLMS.Server.Data.Interfaces;

public interface IHasTenant
{
    Guid TenantId { get; set; }
}