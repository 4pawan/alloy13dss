using System.Collections;
using alloy13dss.Models.Forms;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Data;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models;

namespace alloy13dss.Business.Forms;

public class DummyFormSubmissionAnswerStore(
    IFormDataRepository formDataRepository,
    DataSubmissionService dataSubmissionService,
    IContentLoader contentLoader) : IDummyFormSubmissionAnswerStore
{
    public IReadOnlyDictionary<string, string> GetAnswers(Guid submissionId, DummyFormContainerBlock form)
    {
        var submission = LoadSubmission(submissionId, form);
        var answers = ToValueDictionary(submission);

        foreach (var question in LoadQuestions(form))
        {
            if (string.IsNullOrWhiteSpace(question.BranchingKey))
            {
                continue;
            }

            var value = GetFieldKeys(question)
                .Select(key => answers.TryGetValue(key, out var answer) ? answer : null)
                .FirstOrDefault(answer => answer != null);

            if (value != null)
            {
                answers[question.BranchingKey] = value;
            }
        }

        return answers;
    }

    public void SaveAnswer(Guid submissionId, DummyFormContainerBlock form, IContentData element, string value)
    {
        var submission = LoadSubmission(submissionId, form) ?? CreateSubmission(submissionId);
        var data = EnsureWritableData(submission);

        foreach (var key in GetFieldKeys(element).Take(1))
        {
            data[key] = value ?? string.Empty;
        }

        StoreSubmission(submissionId, form, submission);
    }

    public void ClearAnswers(Guid submissionId, DummyFormContainerBlock form, IEnumerable<ContentReference> elementLinks)
    {
        var submission = LoadSubmission(submissionId, form);
        if (submission?.Data == null)
        {
            return;
        }

        var data = EnsureWritableData(submission);

        foreach (var elementLink in elementLinks.Where(link => !ContentReference.IsNullOrEmpty(link)))
        {
            if (!contentLoader.TryGet(elementLink, out IContentData element))
            {
                continue;
            }

            foreach (var key in GetFieldKeys(element))
            {
                data.Remove(key);
            }
        }

        StoreSubmission(submissionId, form, submission);
    }

    private Submission LoadSubmission(Guid submissionId, DummyFormContainerBlock form)
    {
        var identity = CreateFormIdentity(form);
        if (identity.Guid == Guid.Empty || submissionId == Guid.Empty)
        {
            return null;
        }

        var ids = new[] { submissionId.ToString("D"), submissionId.ToString("N") };
        return formDataRepository.GetSubmissionData(identity, ids).FirstOrDefault();
    }

    private void StoreSubmission(Guid submissionId, DummyFormContainerBlock form, Submission submission)
    {
        var identity = CreateFormIdentity(form);
        if (identity.Guid == Guid.Empty || submissionId == Guid.Empty)
        {
            return;
        }

        dataSubmissionService.StoreSubmissionData(submissionId.ToString("D"), identity, submission);
    }

    private static Submission CreateSubmission(Guid submissionId)
    {
        return new Submission
        {
            Id = submissionId.ToString("D"),
            Data = new Dictionary<string, object>()
        };
    }

    private static IDictionary<string, object> EnsureWritableData(Submission submission)
    {
        if (submission.Data is Dictionary<string, object> dictionary)
        {
            return dictionary;
        }

        submission.Data = submission.Data?.ToDictionary(x => x.Key, x => x.Value)
            ?? new Dictionary<string, object>();

        return submission.Data;
    }

    private static Dictionary<string, string> ToValueDictionary(Submission submission)
    {
        if (submission?.Data == null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return submission.Data.ToDictionary(
            x => x.Key,
            x => ToStringValue(x.Value),
            StringComparer.OrdinalIgnoreCase);
    }

    private IList<DummyQuestionElementBlock> LoadQuestions(DummyFormContainerBlock form)
    {
        var questions = new List<DummyQuestionElementBlock>();

        if (form.ElementsArea?.Items == null)
        {
            return questions;
        }

        foreach (var item in form.ElementsArea.Items)
        {
            if (!ContentReference.IsNullOrEmpty(item.ContentLink) &&
                contentLoader.TryGet(item.ContentLink, out DummyQuestionElementBlock question))
            {
                questions.Add(question);
            }
        }

        return questions;
    }

    private static IEnumerable<string> GetFieldKeys(IContentData element)
    {
        if (element is ElementBlockBase elementBlock)
        {
            if (!string.IsNullOrWhiteSpace(elementBlock.FormElement?.ElementName))
            {
                yield return elementBlock.FormElement.ElementName;
            }

            if (elementBlock.FormElement?.Guid is { } elementGuid && elementGuid != Guid.Empty)
            {
                yield return elementGuid.ToString("D");
                yield return elementGuid.ToString("N");
            }
        }

        if (element is IContent content)
        {
            yield return content.ContentLink.ID.ToString();
        }

        if (element is DummyQuestionElementBlock question && !string.IsNullOrWhiteSpace(question.BranchingKey))
        {
            yield return question.BranchingKey;
        }
    }

    private static FormIdentity CreateFormIdentity(DummyFormContainerBlock form)
    {
        return new FormIdentity
        {
            Guid = form.Form?.FormGuid ?? Guid.Empty,
            Language = form.Form?.Language ?? string.Empty
        };
    }

    private static string ToStringValue(object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is string text)
        {
            return text;
        }

        if (value is IEnumerable values)
        {
            return string.Join(",", values.Cast<object>().Select(x => x?.ToString()).Where(x => x != null));
        }

        return value.ToString();
    }
}
