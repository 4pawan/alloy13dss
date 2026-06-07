using System.ComponentModel.DataAnnotations;
using alloy13dss.Models.Blocks;

namespace alloy13dss.Models.Pages;

[SiteContentType(
    DisplayName = "Settings page",
    Description = "Shared configuration for form tools and site-level integrations.",
    GUID = "C6FA8E52-00D9-47FE-84CE-4020D47038C7",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl]
[AvailableContentTypes(Availability.None)]
public class SettingsPage : SitePageData
{
    [Display(
        Name = "Form tools",
        Description = "Reusable form tool settings blocks. Pages can select one by key.",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    [AllowedTypes(typeof(FormToolSettingsBlock))]
    public virtual ContentArea FormTools { get; set; }
}
