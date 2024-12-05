namespace SaasLMS.Shared.Models.Enums;

public enum ReviewStatus
{
    Pending,
    Published,
    Rejected,
    Flagged
}

public enum DiscussionStatus
{
    Active,
    Locked,
    Hidden,
    Deleted
}

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}

public enum ThreadStatus
{
    Active,
    Muted,
    Archived,
    Left
}