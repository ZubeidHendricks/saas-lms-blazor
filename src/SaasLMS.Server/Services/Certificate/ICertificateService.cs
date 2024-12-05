using SaasLMS.Shared.Models.Enrollment;
using SaasLMS.Shared.DTOs;

namespace SaasLMS.Server.Services.Certificate;

public interface ICertificateService
{
    Task<CertificateDTO> GenerateCertificateAsync(Enrollment enrollment);
    Task<CertificateDTO> GetCertificateAsync(Guid certificateId);
    Task<bool> ValidateCertificateAsync(string certificateNumber);
    Task<IEnumerable<CertificateDTO>> GetUserCertificatesAsync(Guid userId);
    Task<byte[]> DownloadCertificateAsync(Guid certificateId, string format = "pdf");
}