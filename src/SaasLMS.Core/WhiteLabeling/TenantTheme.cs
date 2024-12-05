namespace SaasLMS.Core.WhiteLabeling;

public class TenantTheme
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    
    // Brand Colors
    public string PrimaryColor { get; set; } = "#3B82F6";
    public string SecondaryColor { get; set; } = "#10B981";
    public string AccentColor { get; set; } = "#6366F1";
    public string TextColor { get; set; } = "#1F2937";
    public string BackgroundColor { get; set; } = "#FFFFFF";
    
    // Typography
    public string PrimaryFont { get; set; } = "Inter";
    public string HeadingFont { get; set; } = "Inter";
    public string FontCdn { get; set; }
    
    // Brand Assets
    public string LogoUrl { get; set; }
    public string FaviconUrl { get; set; }
    public string LoginBackgroundUrl { get; set; }
    
    // Custom Styles
    public Dictionary<string, string> CustomCss { get; set; } = new();
    
    // Layout Settings
    public LayoutSettings Layout { get; set; } = new();
    
    // Email Template Settings
    public EmailTemplateSettings EmailTemplates { get; set; } = new();
}

public class LayoutSettings
{
    public string ContainerWidth { get; set; } = "1280px";
    public string HeaderStyle { get; set; } = "default";
    public string FooterStyle { get; set; } = "default";
    public bool EnableSidebar { get; set; } = true;
    public string SidebarPosition { get; set; } = "left";
    public Dictionary<string, bool> ComponentVisibility { get; set; } = new();
}

public class EmailTemplateSettings
{
    public string HeaderLogoUrl { get; set; }
    public string FooterText { get; set; }
    public string SignatureText { get; set; }
    public Dictionary<string, string> Templates { get; set; } = new();
    public Dictionary<string, string> Styles { get; set; } = new();
}