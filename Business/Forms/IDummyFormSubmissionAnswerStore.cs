using alloy13dss.Models.Forms;

namespace alloy13dss.Business.Forms;

public interface IDummyFormSubmissionAnswerStore
{
    IReadOnlyDictionary<string, string> GetAnswers(Guid submissionId, DummyFormContainerBlock form);

    void SaveAnswer(Guid submissionId, DummyFormContainerBlock form, IContentData element, string value);

    void ClearAnswers(Guid submissionId, DummyFormContainerBlock form, IEnumerable<ContentReference> elementLinks);
}
