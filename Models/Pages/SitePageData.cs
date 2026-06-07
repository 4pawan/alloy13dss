using System.ComponentModel.DataAnnotations;
using alloy13dss.Business.Rendering;
using alloy13dss.Models.Forms;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

namespace alloy13dss.Models.Pages;

/// <summary>
/// Base class for all page types
/// </summary>
public abstract class SitePageData : PageData, ICustomCssInContentArea
{
    [Display(
        GroupName = Globals.GroupNames.MetaData,
        Order = 100)]
    [CultureSpecific]
    public virtual string MetaTitle
    {
        get
        {
            var metaTitle = this.GetPropertyValue(p => p.MetaTitle);

            // Use explicitly set meta title, otherwise fall back to page name
            return !string.IsNullOrWhiteSpace(metaTitle)
                   ? metaTitle
                   : Name;
        }
        set => this.SetPropertyValue(p => p.MetaTitle, value);
    }

    [Display(
        GroupName = Globals.GroupNames.MetaData,
        Order = 200)]
    [CultureSpecific]
    [BackingType(typeof(PropertyStringList))]
    public virtual IList<string> MetaKeywords { get; set; }

    [Display(
        GroupName = Globals.GroupNames.MetaData,
        Order = 300)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string MetaDescription { get; set; }

    [Display(
        GroupName = Globals.GroupNames.MetaData,
        Order = 400)]
    [CultureSpecific]
    public virtual bool DisableIndexing { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Order = 100)]
    [UIHint(UIHint.Image)]
    public virtual ContentReference PageImage { get; set; }

    [Display(
        GroupName = SystemTabNames.Content,
        Order = 200)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string TeaserText
    {
        get
        {
            var teaserText = this.GetPropertyValue(p => p.TeaserText);

            // Use explicitly set teaser text, otherwise fall back to description
            return !string.IsNullOrWhiteSpace(teaserText)
                ? teaserText
                : MetaDescription;
        }
        set => this.SetPropertyValue(p => p.TeaserText, value);
    }

    [Display(
        GroupName = SystemTabNames.Settings,
        Order = 200)]
    [CultureSpecific]
    public virtual bool HideSiteHeader { get; set; }

    [Display(
        GroupName = SystemTabNames.Settings,
        Order = 300)]
    [CultureSpecific]
    public virtual bool HideSiteFooter { get; set; }

    [Display(
        Name = "Settings page",
        Description = "Optional settings source for reusable form tools.",
        GroupName = SystemTabNames.Settings,
        Order = 400)]
    [AllowedTypes(typeof(SettingsPage))]
    public virtual ContentReference SettingsPageLink { get; set; }

    [Display(
        Name = "Form tool key",
        Description = "Key of the tool definition on the selected settings page.",
        GroupName = SystemTabNames.Settings,
        Order = 410)]
    public virtual string FormToolKey { get; set; }

    [Display(
        Name = "Direct form container block",
        Description = "Optional page-level override. If set, this block is used instead of the settings page tool key.",
        GroupName = SystemTabNames.Settings,
        Order = 420)]
    [AllowedTypes(typeof(DummyFormContainerBlock))]
    public virtual ContentReference DirectFormContainerBlock { get; set; }

    public string ContentAreaCssClass => "teaserblock";
}
