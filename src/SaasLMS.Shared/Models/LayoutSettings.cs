namespace SaasLMS.Shared.Models;

public class LayoutSettings
{
    public bool ShowHeader { get; set; } = true;
    public bool ShowFooter { get; set; } = true;
    public NavigationSettings Navigation { get; set; } = new();
    public Dictionary<string, bool> FeatureToggles { get; set; } = new();
}