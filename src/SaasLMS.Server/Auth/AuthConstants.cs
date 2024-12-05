namespace SaasLMS.Server.Auth;

public static class AuthConstants
{
    public static class Roles
    {
        public const string SystemAdmin = "SystemAdmin";
        public const string TenantAdmin = "TenantAdmin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";
    }

    public static class Policies
    {
        public const string RequireSystemAdmin = "RequireSystemAdmin";
        public const string RequireTenantAdmin = "RequireTenantAdmin";
        public const string RequireInstructor = "RequireInstructor";
        public const string RequireTenantAccess = "RequireTenantAccess";
    }

    public static class Claims
    {
        public const string TenantId = "tenant_id";
        public const string Permission = "permission";
    }
}