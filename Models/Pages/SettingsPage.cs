using System.ComponentModel.DataAnnotations;
using alloy13dss.Models.Blocks;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

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
        Name = "reCAPTCHA v3 site key",
        Description = "Public site key used by dummy form journeys.",
        GroupName = SystemTabNames.Content,
        Order = 80)]
    public virtual string RecaptchaSiteKey { get; set; }

    [Display(
        Name = "reCAPTCHA v3 secret key",
        Description = "Secret key used server-side to validate dummy form journey submissions.",
        GroupName = SystemTabNames.Content,
        Order = 90)]
    [ScaffoldColumn(false)]
    public virtual string RecaptchaSecretKey { get; set; }

    [Display(
        Name = "reCAPTCHA score threshold",
        Description = "Minimum accepted v3 score for each dummy form step.",
        GroupName = SystemTabNames.Content,
        Order = 95)]
    [Range(0, 1)]
    public virtual double RecaptchaScoreThreshold { get; set; }

    [Display(
        Name = "Form tools",
        Description = "Reusable form tool settings blocks. Pages can select one by key.",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    [AllowedTypes(typeof(FormToolSettingsBlock))]
    public virtual ContentArea FormTools { get; set; }

    [Display(
        Name = "Footer links",
        Description = "Links rendered in the site and dummy tool footers.",
        GroupName = SystemTabNames.Content,
        Order = 110)]
    public virtual LinkItemCollection FooterLinks { get; set; }

    [Display(
        Name = "GTM script",
        Description = "Google Tag Manager script rendered in the page head.",
        GroupName = SystemTabNames.Content,
        Order = 120)]
    [UIHint(UIHint.Textarea)]
    public virtual string GtmScript { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        RecaptchaScoreThreshold = 0.5;
    }
}
