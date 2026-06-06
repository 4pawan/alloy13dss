using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy reCAPTCHA v3",
    Description = "Hidden per-step captcha token element for dummy form journeys.",
    GUID = "64287e78-1d68-4a8c-9f02-33647b81daaf",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyRecaptchaV3ElementBlock : PredefinedHiddenElementBlock
{
    [Display(
        Name = "Action",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    public virtual string Action { get; set; }

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);

        Action = "dummy_form_step";
    }
}
