using EPiServer.Data.Dynamic;

namespace alloy13dss.Business.Forms;

public class DummyFormJourneyStateRepository : IDummyFormJourneyStateRepository
{
    private readonly DynamicDataStore _store;

    public DummyFormJourneyStateRepository()
    {
        _store = DynamicDataStoreFactory.Instance.GetStore(typeof(DummyFormJourneyState))
            ?? DynamicDataStoreFactory.Instance.CreateStore(typeof(DummyFormJourneyState));
    }

    public DummyFormJourneyState Get(Guid submissionId)
    {
        return _store.Items<DummyFormJourneyState>()
            .FirstOrDefault(x => x.SubmissionId == submissionId);
    }

    public void Save(DummyFormJourneyState state)
    {
        state.UpdatedUtc = DateTime.UtcNow;
        _store.Save(state);
    }

    public void ResetBranch(DummyFormJourneyState state, ContentReference branchTarget)
    {
        var targetIndex = state.VisitedElements.FindIndex(x => ContentReferenceEquals(x, branchTarget));

        if (targetIndex < 0)
        {
            state.VisitedElements.Add(branchTarget);
            state.CurrentElement = branchTarget;
            return;
        }

        state.VisitedElements = state.VisitedElements.Take(targetIndex + 1).ToList();
        state.CurrentElement = branchTarget;
    }

    private static bool ContentReferenceEquals(ContentReference left, ContentReference right)
    {
        return !ContentReference.IsNullOrEmpty(left)
            && !ContentReference.IsNullOrEmpty(right)
            && left.ID == right.ID
            && left.ProviderName == right.ProviderName;
    }
}
