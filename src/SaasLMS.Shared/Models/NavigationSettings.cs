namespace SaasLMS.Shared.Models;

public class NavigationSettings
{
    public bool ShowBreadcrumbs { get; set; } = true;
    public bool ShowSidebar { get; set; } = true;
    public string SidebarPosition { get; set; } = "left";
    public List<string> DisabledMenuItems { get; set; } = new();
}