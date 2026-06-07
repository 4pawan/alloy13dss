using alloy13dss.Models.Forms;
using EPiServer.Forms.Core;

namespace alloy13dss.Models.ViewModels;

public class DummyFormContainerViewModel
{
    public DummyFormContainerBlock Form { get; set; }

    public ContentReference FormContentLink { get; set; }

    public ContentReference SettingsPageLink { get; set; }

    public string RecaptchaSiteKey { get; set; }

    public ElementBlockBase ActiveElement { get; set; }

    public string ActiveElementTitle { get; set; }

    public Guid SubmissionId { get; set; }

    public ContentReference ActiveElementLink { get; set; }

    public bool CanGoBack { get; set; }

    public bool IsComplete { get; set; }

    public string Message { get; set; }
}
