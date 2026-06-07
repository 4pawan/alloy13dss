using System.Collections.Concurrent;
using alloy13dss.Models.Forms;

namespace alloy13dss.Business.Forms;

public class DummyFormSubmissionAnswerStore : IDummyFormSubmissionAnswerStore
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, string>> _answers = new();

    public IReadOnlyDictionary<string, string> GetAnswers(Guid submissionId)
    {
        return _answers.TryGetValue(submissionId, out var answers)
            ? answers.ToDictionary(x => x.Key, x => x.Value)
            : new Dictionary<string, string>();
    }

    public void SaveAnswer(Guid submissionId, IContentData element, string value)
    {
        var answers = _answers.GetOrAdd(submissionId, _ => new ConcurrentDictionary<string, string>());
        var submittedValue = value ?? string.Empty;

        if (element is IContent content)
        {
            answers[GetKey(content.ContentLink)] = submittedValue;
        }

        if (element is DummyQuestionElementBlock question && !string.IsNullOrWhiteSpace(question.BranchingKey))
        {
            answers[question.BranchingKey] = submittedValue;
        }
    }

    public void ClearAnswers(Guid submissionId, IEnumerable<ContentReference> elementLinks)
    {
        if (!_answers.TryGetValue(submissionId, out var answers))
        {
            return;
        }

        foreach (var elementLink in elementLinks)
        {
            answers.TryRemove(GetKey(elementLink), out _);
        }
    }

    private static string GetKey(ContentReference elementLink) => elementLink?.ID.ToString() ?? string.Empty;
}
