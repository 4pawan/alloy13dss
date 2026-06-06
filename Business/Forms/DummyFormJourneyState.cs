using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace alloy13dss.Business.Forms;

public class DummyFormJourneyState : IDynamicData
{
    public Identity Id { get; set; }

    public Guid SubmissionId { get; set; }

    public ContentReference FormContentLink { get; set; }

    public ContentReference CurrentElement { get; set; }

    public List<ContentReference> VisitedElements { get; set; } = [];

    public DateTime UpdatedUtc { get; set; }
}
