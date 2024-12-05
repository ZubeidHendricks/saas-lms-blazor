namespace SaasLMS.Core.Offline;

public class CourseBasicInfo
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal OfflineStorageSize { get; set; }
    public DateTime LastSynced { get; set; }
    public bool IsSynced { get; set; }
}