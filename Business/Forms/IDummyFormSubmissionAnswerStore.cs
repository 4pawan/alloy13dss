namespace alloy13dss.Business.Forms;

public interface IDummyFormSubmissionAnswerStore
{
    IReadOnlyDictionary<string, string> GetAnswers(Guid submissionId);

    void SaveAnswer(Guid submissionId, IContentData element, string value);

    void ClearAnswers(Guid submissionId, IEnumerable<ContentReference> elementLinks);
}
