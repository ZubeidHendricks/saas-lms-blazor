namespace SaasLMS.Shared.Models;

public class ThemeConfiguration
{
    public string PrimaryColor { get; set; } = "#007bff";
    public string SecondaryColor { get; set; } = "#6c757d";
    public string LogoUrl { get; set; } = string.Empty;
    public string FaviconUrl { get; set; } = string.Empty;
    public Dictionary<string, string> CustomCss { get; set; } = new();
    public Dictionary<string, string> Translations { get; set; } = new();
    public LayoutSettings Layout { get; set; } = new();
}