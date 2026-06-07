using System.ComponentModel.DataAnnotations;
using alloy13dss.Business.Rendering;

namespace alloy13dss.Models.Pages;

/// <summary>
/// Root container for a reusable tool section. Child pages inherit settings from this page.
/// </summary>
[SiteContentType(
    DisplayName = "Root folder",
    Description = "Root folder for a reusable tool. Set the settings page here and place tool pages below it.",
    GUID = "8F4C3A7F-3434-4DB9-8DB7-5F7E7A6C7581",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl]
public class RootFolder : SitePageData, IContainerPage
{
    [Display(
        Name = "Settings page",
        Description = "Shared settings source for all tool pages below this root folder.",
        GroupName = SystemTabNames.Settings,
        Order = 400)]
    [AllowedTypes(typeof(SettingsPage))]
    public virtual ContentReference SettingsPageLink { get; set; }
}
