using System.ComponentModel.DataAnnotations;

namespace alloy13dss.Models.Pages;

/// <summary>
/// Minimal page type for rendering reusable block-based tool/content pages.
/// </summary>
[SiteContentType(
    DisplayName = "Content page",
    Description = "A block-based page that uses the dummy tool layout.",
    GUID = "4E2B35B7-8F38-40C1-929D-F0F0D0F5FB52",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl]
public class ContentPage : SitePageData
{
    [Display(
        Name = "Content area",
        Description = "Add content blocks, form tool blocks, or other reusable components for this page.",
        GroupName = SystemTabNames.Content,
        Order = 310)]
    [CultureSpecific]
    public virtual ContentArea ContentArea { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        HideSiteFooter = false;
        HideSiteHeader = false;
    }
}
