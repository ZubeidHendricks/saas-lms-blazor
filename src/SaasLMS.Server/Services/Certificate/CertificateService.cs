using SaasLMS.Shared.Models.Enrollment;
using SaasLMS.Shared.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SaasLMS.Server.Services.Certificate;

public class CertificateService : ICertificateService
{
    private readonly IStorageService _storageService;
    private readonly ITenantService _tenantService;
    private readonly ILogger<CertificateService> _logger;

    public CertificateService(
        IStorageService storageService,
        ITenantService tenantService,
        ILogger<CertificateService> logger)
    {
        _storageService = storageService;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task<CertificateDTO> GenerateCertificateAsync(Enrollment enrollment)
    {
        try
        {
            var certificateData = new CertificateData
            {
                CertificateNumber = GenerateCertificateNumber(),
                StudentName = enrollment.User.FirstName + " " + enrollment.User.LastName,
                CourseName = enrollment.Course.Title,
                CompletionDate = enrollment.CompletedAt!.Value.ToString("MMMM dd, yyyy"),
                InstructorName = enrollment.Course.Instructor.Name,
                InstructorTitle = enrollment.Course.Instructor.Title,
                TenantLogo = _tenantService.CurrentTenant.ThemeConfig.LogoUrl,
                TenantName = _tenantService.CurrentTenant.Name
            };

            // Generate PDF using QuestPDF
            var document = new CertificateDocument(certificateData);
            var pdfBytes = document.GeneratePdf();

            // Upload to storage
            var certificateUrl = await _storageService.UploadFileAsync(
                pdfBytes,
                $"certificates/{enrollment.Id}/{certificateData.CertificateNumber}.pdf");

            // Update enrollment
            enrollment.IsCertificateIssued = true;
            enrollment.CertificateUrl = certificateUrl;
            enrollment.CertificateIssuedAt = DateTime.UtcNow;

            return new CertificateDTO
            {
                CertificateId = Guid.NewGuid(), // You might want to store this in a certificates table
                CertificateNumber = certificateData.CertificateNumber,
                StudentName = certificateData.StudentName,
                CourseName = certificateData.CourseName,
                CompletionDate = enrollment.CompletedAt.Value,
                IssueDate = enrollment.CertificateIssuedAt.Value,
                CertificateUrl = certificateUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating certificate for enrollment {EnrollmentId}", enrollment.Id);
            throw;
        }
    }

    private string GenerateCertificateNumber()
    {
        return $"{_tenantService.CurrentTenant.Id.ToString().Substring(0, 8)}-" +
               $"{DateTime.UtcNow:yyyyMMdd}-" +
               $"{Guid.NewGuid().ToString().Substring(0, 8)}".ToUpper();
    }

    private class CertificateDocument : IDocument
    {
        private readonly CertificateData _data;

        public CertificateDocument(CertificateData data)
        {
            _data = data;
        }

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            // Header with logo
                            x.Item().Row(row =>
                            {
                                row.RelativeItem().Image(_data.TenantLogo);
                            });

                            // Certificate title
                            x.Item().PaddingVertical(1, Unit.Centimetre)
                                .Text("Certificate of Completion")
                                .FontSize(32)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);

                            // Main content
                            x.Item().PaddingVertical(1, Unit.Centimetre)
                                .Text(text =>
                                {
                                    text.Span("This is to certify that ");
                                    text.Span(_data.StudentName).Bold();
                                    text.Span(" has successfully completed the course ");
                                    text.Span(_data.CourseName).Bold();
                                    text.Span(" on ");
                                    text.Span(_data.CompletionDate).Bold();
                                });

                            // Signatures
                            x.Item().PaddingTop(2, Unit.Centimetre)
                                .Row(row =>
                                {
                                    row.RelativeItem().Column(column =>
                                    {
                                        column.Item().Text(_data.InstructorName)
                                            .Bold();
                                        column.Item().Text(_data.InstructorTitle);
                                    });
                                });

                            // Certificate number
                            x.Item().AlignRight()
                                .Text($"Certificate No: {_data.CertificateNumber}")
                                .FontSize(8);
                        });
                });
        }

        private class CertificateData
        {
            public string CertificateNumber { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public string CompletionDate { get; set; } = string.Empty;
            public string InstructorName { get; set; } = string.Empty;
            public string InstructorTitle { get; set; } = string.Empty;
            public string TenantLogo { get; set; } = string.Empty;
            public string TenantName { get; set; } = string.Empty;
        }
    }
}