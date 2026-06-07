using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.SpecializedProperties;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy question",
    Description = "Question element intended for one-at-a-time dummy form journeys.",
    GUID = "C15E2CCF-329C-4261-9020-9A8AA3B62D2B",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyQuestionElementBlock : TextboxElementBlock
{
    [Display(
        Name = "Source key",
        Description = "Stable key used by other question rules to read this question's submitted response.",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    public virtual string BranchingKey { get; set; }

    [Display(
        Name = "Always redirect to source key",
        Description = "Optional source key of the question that should always be shown after this question is submitted.",
        GroupName = SystemTabNames.Content,
        Order = 105)]
    public virtual string AlwaysRedirectToSourceKey { get; set; }

    [Display(
        Name = "Visibility rules",
        Description = "Rules that decide whether this question should be shown. All rules must match.",
        GroupName = SystemTabNames.Content,
        Order = 120)]
    [BackingType(typeof(PropertyDummyQuestionVisibilityRuleList))]
    public virtual IList<DummyQuestionVisibilityRule> Rules { get; set; }
}

public class DummyQuestionVisibilityRule
{
    [Display(
        Name = "Submitted question key",
        Description = "Branching key of the previously submitted question to check.",
        Order = 10)]
    public virtual string QuestionKey { get; set; }

    [Display(Name = "Operator", Order = 20)]
    public virtual DummyQuestionRuleOperator Operator { get; set; }

    [Display(Name = "Value", Order = 30)]
    public virtual string Value { get; set; }
}

public enum DummyQuestionRuleOperator
{
    Equals,
    NotEquals,
    Contains,
    IsEmpty,
    IsNotEmpty
}

[PropertyDefinitionType]
public class PropertyDummyQuestionVisibilityRuleList : PropertyList<DummyQuestionVisibilityRule>
{
}
