using alloy13dss.Models.Forms;

namespace alloy13dss.Business.Forms;

public class DummyFormBranchEvaluator : IDummyFormBranchEvaluator
{
    public ContentReference ResolveNextElement(
        DummyFormContainerBlock form,
        DummyFormJourneyState state,
        IReadOnlyDictionary<string, string> submissionAnswers)
    {
        var currentRule = form.JourneyRules?.FirstOrDefault(x => SameContent(x.Element, state.CurrentElement));

        if (currentRule == null)
        {
            return FirstEligibleElement(form, submissionAnswers);
        }

        var matchedBranch = currentRule.Branches?
            .FirstOrDefault(branch => AreSatisfied(branch.Conditions, submissionAnswers));

        if (matchedBranch != null && !ContentReference.IsNullOrEmpty(matchedBranch.TargetElement))
        {
            return matchedBranch.TargetElement;
        }

        if (!ContentReference.IsNullOrEmpty(currentRule.DefaultNextElement))
        {
            return FirstEligibleElement(form, submissionAnswers, currentRule.DefaultNextElement);
        }

        var currentIndex = form.JourneyRules?.ToList().FindIndex(x => SameContent(x.Element, currentRule.Element)) ?? -1;

        return FirstEligibleElement(form, submissionAnswers, startIndex: currentIndex + 1);
    }

    private static ContentReference FirstEligibleElement(
        DummyFormContainerBlock form,
        IReadOnlyDictionary<string, string> submissionAnswers,
        ContentReference preferredElement = null,
        int startIndex = 0)
    {
        var rules = form.JourneyRules?.ToList() ?? [];

        if (!ContentReference.IsNullOrEmpty(preferredElement))
        {
            var preferred = rules.FirstOrDefault(x => SameContent(x.Element, preferredElement));
            if (preferred != null && AreSatisfied(preferred.Conditions, submissionAnswers))
            {
                return preferred.Element;
            }
        }

        foreach (var rule in rules.Skip(Math.Max(startIndex, 0)))
        {
            if (!ContentReference.IsNullOrEmpty(rule.Element) && AreSatisfied(rule.Conditions, submissionAnswers))
            {
                return rule.Element;
            }
        }

        return ContentReference.EmptyReference;
    }

    private static bool AreSatisfied(IList<DummyFormCondition> conditions, IReadOnlyDictionary<string, string> submissionAnswers)
    {
        return conditions == null || conditions.Count == 0 || conditions.All(condition => IsSatisfied(condition, submissionAnswers));
    }

    private static bool IsSatisfied(DummyFormCondition condition, IReadOnlyDictionary<string, string> submissionAnswers)
    {
        var key = condition.QuestionElement?.ID.ToString();
        var submittedValue = string.Empty;
        var hasValue = key != null && submissionAnswers.TryGetValue(key, out submittedValue);
        submittedValue ??= string.Empty;
        var expected = condition.Value ?? string.Empty;

        return condition.Operator switch
        {
            DummyFormConditionOperator.Equals => string.Equals(submittedValue, expected, StringComparison.OrdinalIgnoreCase),
            DummyFormConditionOperator.NotEquals => !string.Equals(submittedValue, expected, StringComparison.OrdinalIgnoreCase),
            DummyFormConditionOperator.Contains => submittedValue.Contains(expected, StringComparison.OrdinalIgnoreCase),
            DummyFormConditionOperator.IsEmpty => !hasValue || string.IsNullOrWhiteSpace(submittedValue),
            DummyFormConditionOperator.IsNotEmpty => hasValue && !string.IsNullOrWhiteSpace(submittedValue),
            _ => false
        };
    }

    private static bool SameContent(ContentReference left, ContentReference right)
    {
        return !ContentReference.IsNullOrEmpty(left)
            && !ContentReference.IsNullOrEmpty(right)
            && left.ID == right.ID
            && left.ProviderName == right.ProviderName;
    }
}
