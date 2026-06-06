namespace alloy13dss.Business.Forms;

public interface IDummyFormJourneyStateRepository
{
    DummyFormJourneyState Get(Guid submissionId);

    void Save(DummyFormJourneyState state);

    void ResetBranch(DummyFormJourneyState state, ContentReference branchTarget);
}
