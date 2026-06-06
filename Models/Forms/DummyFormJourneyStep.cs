using System.ComponentModel.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace alloy13dss.Models.Forms;

public class DummyFormJourneyStep
{
    [Display(Name = "Element", Order = 10)]
    [AllowedTypes(typeof(EPiServer.Forms.Core.ElementBlockBase))]
    public virtual ContentReference Element { get; set; }

    [Display(Name = "Optional", Order = 20)]
    public virtual bool Optional { get; set; }

    [Display(Name = "Show only when", Order = 30)]
    [BackingType(typeof(PropertyDummyFormConditionList))]
    public virtual IList<DummyFormCondition> Conditions { get; set; }

    [Display(Name = "Branches", Order = 40)]
    [BackingType(typeof(PropertyDummyFormBranchList))]
    public virtual IList<DummyFormBranch> Branches { get; set; }

    [Display(Name = "Default next element", Order = 50)]
    [AllowedTypes(typeof(EPiServer.Forms.Core.ElementBlockBase))]
    public virtual ContentReference DefaultNextElement { get; set; }
}

public class DummyFormCondition
{
    [Display(Name = "Question element", Order = 10)]
    [AllowedTypes(typeof(EPiServer.Forms.Core.ElementBlockBase))]
    public virtual ContentReference QuestionElement { get; set; }

    [Display(Name = "Operator", Order = 20)]
    public virtual DummyFormConditionOperator Operator { get; set; }

    [Display(Name = "Value", Order = 30)]
    public virtual string Value { get; set; }
}

public class DummyFormBranch
{
    [Display(Name = "When", Order = 10)]
    [BackingType(typeof(PropertyDummyFormConditionList))]
    public virtual IList<DummyFormCondition> Conditions { get; set; }

    [Display(Name = "Go to element", Order = 20)]
    [AllowedTypes(typeof(EPiServer.Forms.Core.ElementBlockBase))]
    public virtual ContentReference TargetElement { get; set; }
}

public enum DummyFormConditionOperator
{
    Equals,
    NotEquals,
    Contains,
    IsEmpty,
    IsNotEmpty
}
