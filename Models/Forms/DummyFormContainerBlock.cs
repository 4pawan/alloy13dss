using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Web;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy form container",
    Description = "Renders one Forms element at a time with editor-managed branching.",
    GUID = "45750D5D-8011-43F9-86F5-A1F991FF57A5",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyFormContainerBlock : FormContainerBlock
{
    [Display(
        Name = "reCAPTCHA v3 site key",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    public virtual string RecaptchaSiteKey { get; set; }

    [Display(
        Name = "reCAPTCHA v3 secret key",
        GroupName = SystemTabNames.Content,
        Order = 110)]
    [ScaffoldColumn(false)]
    public virtual string RecaptchaSecretKey { get; set; }

    [Display(
        Name = "reCAPTCHA score threshold",
        GroupName = SystemTabNames.Content,
        Order = 120)]
    [Range(0, 1)]
    public virtual double RecaptchaScoreThreshold { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        RecaptchaScoreThreshold = 0.5;
    }
}
