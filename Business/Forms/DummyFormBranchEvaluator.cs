using alloy13dss.Models.Forms;

namespace alloy13dss.Business.Forms;

public class DummyFormBranchEvaluator(IContentLoader contentLoader) : IDummyFormBranchEvaluator
{
    public ContentReference ResolveFirstElement(DummyFormContainerBlock form)
    {
        var questions = LoadQuestions(form);

        return GetContentLink(questions.FirstOrDefault())
            ?? ContentReference.EmptyReference;
    }

    public ContentReference ResolveNextElement(
        DummyFormContainerBlock form,
        DummyFormJourneyState state,
        IReadOnlyDictionary<string, string> submissionAnswers)
    {
        var questions = LoadQuestions(form);
        var currentIndex = IndexOf(questions, state.CurrentElement);
        var currentQuestion = currentIndex >= 0 ? questions[currentIndex] : null;

        if (!string.IsNullOrWhiteSpace(currentQuestion?.AlwaysRedirectToSourceKey))
        {
            var redirectQuestion = questions.FirstOrDefault(question =>
                string.Equals(
                    question.BranchingKey,
                    currentQuestion.AlwaysRedirectToSourceKey,
                    StringComparison.OrdinalIgnoreCase));

            if (redirectQuestion != null)
            {
                return GetContentLink(redirectQuestion);
            }
        }

        foreach (var question in questions.Skip(Math.Max(currentIndex + 1, 0)))
        {
            if (AreSatisfied(question.Rules, submissionAnswers))
            {
                return GetContentLink(question);
            }
        }

        return ContentReference.EmptyReference;
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
            if (ContentReference.IsNullOrEmpty(item.ContentLink))
            {
                continue;
            }

            if (contentLoader.TryGet(item.ContentLink, out DummyQuestionElementBlock question))
            {
                questions.Add(question);
            }
        }

        return questions;
    }

    private static bool AreSatisfied(IList<DummyQuestionVisibilityRule> rules, IReadOnlyDictionary<string, string> submissionAnswers)
    {
        return rules == null || rules.Count == 0 || rules.All(rule => IsSatisfied(rule, submissionAnswers));
    }

    private static bool IsSatisfied(DummyQuestionVisibilityRule rule, IReadOnlyDictionary<string, string> submissionAnswers)
    {
        var key = rule.QuestionKey ?? string.Empty;
        var submittedValue = string.Empty;
        var hasValue = !string.IsNullOrWhiteSpace(key) && submissionAnswers.TryGetValue(key, out submittedValue);
        submittedValue ??= string.Empty;
        var expected = rule.Value ?? string.Empty;

        return rule.Operator switch
        {
            DummyQuestionRuleOperator.Equals => string.Equals(submittedValue, expected, StringComparison.OrdinalIgnoreCase),
            DummyQuestionRuleOperator.NotEquals => !string.Equals(submittedValue, expected, StringComparison.OrdinalIgnoreCase),
            DummyQuestionRuleOperator.Contains => submittedValue.Contains(expected, StringComparison.OrdinalIgnoreCase),
            DummyQuestionRuleOperator.IsEmpty => !hasValue || string.IsNullOrWhiteSpace(submittedValue),
            DummyQuestionRuleOperator.IsNotEmpty => hasValue && !string.IsNullOrWhiteSpace(submittedValue),
            _ => false
        };
    }

    private static int IndexOf(IList<DummyQuestionElementBlock> questions, ContentReference element)
    {
        for (var index = 0; index < questions.Count; index++)
        {
            if (SameContent(GetContentLink(questions[index]), element))
            {
                return index;
            }
        }

        return -1;
    }

    private static bool SameContent(ContentReference left, ContentReference right)
    {
        return !ContentReference.IsNullOrEmpty(left)
            && !ContentReference.IsNullOrEmpty(right)
            && left.ID == right.ID
            && left.ProviderName == right.ProviderName;
    }

    private static ContentReference GetContentLink(IContentData contentData)
    {
        return contentData is IContent content ? content.ContentLink : ContentReference.EmptyReference;
    }
}
