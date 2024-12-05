namespace SaasLMS.Core.Courses;

public class CourseEnrollment
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string StudentId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal AmountPaid { get; set; }
    public string TransactionId { get; set; }
}

public enum EnrollmentStatus
{
    Active,
    Completed,
    Withdrawn,
    Expired
}