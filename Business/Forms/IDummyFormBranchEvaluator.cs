using alloy13dss.Models.Forms;

namespace alloy13dss.Business.Forms;

public interface IDummyFormBranchEvaluator
{
    ContentReference ResolveFirstElement(DummyFormContainerBlock form);

    ContentReference ResolveNextElement(
        DummyFormContainerBlock form,
        DummyFormJourneyState state,
        IReadOnlyDictionary<string, string> submissionAnswers);
}
