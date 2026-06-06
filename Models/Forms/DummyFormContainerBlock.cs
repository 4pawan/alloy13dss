using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Web;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy form container",
    Description = "Renders one Forms element at a time with editor-managed branching.",
    GUID = "45750d5d-8011-43f9-86f5-a1f991ff57a5",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyFormContainerBlock : FormContainerBlock
{
    [Display(
        Name = "Journey rules",
        Description = "Ordered rules that decide which element is shown next.",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    [BackingType(typeof(PropertyDummyFormJourneyStepList))]
    public virtual IList<DummyFormJourneyStep> JourneyRules { get; set; }

    [Display(
        Name = "reCAPTCHA v3 site key",
        GroupName = SystemTabNames.Content,
        Order = 110)]
    public virtual string RecaptchaSiteKey { get; set; }

    [Display(
        Name = "reCAPTCHA v3 secret key",
        GroupName = SystemTabNames.Content,
        Order = 120)]
    [ScaffoldColumn(false)]
    public virtual string RecaptchaSecretKey { get; set; }

    [Display(
        Name = "reCAPTCHA score threshold",
        GroupName = SystemTabNames.Content,
        Order = 130)]
    [Range(0, 1)]
    public virtual double RecaptchaScoreThreshold { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        RecaptchaScoreThreshold = 0.5;
    }
}
