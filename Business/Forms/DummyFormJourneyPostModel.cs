namespace alloy13dss.Business.Forms;

public class DummyFormJourneyPostModel
{
    public ContentReference FormContentLink { get; set; }

    public Guid SubmissionId { get; set; }

    public ContentReference CurrentElementLink { get; set; }

    public string CurrentValue { get; set; }

    public string RecaptchaToken { get; set; }

    public string Command { get; set; }

    public string ReturnUrl { get; set; }
}
