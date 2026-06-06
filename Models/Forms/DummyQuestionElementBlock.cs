using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy question",
    Description = "Question element intended for one-at-a-time dummy form journeys.",
    GUID = "c15e2ccf-329c-4261-9020-9a8aa3b62d2b",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyQuestionElementBlock : TextboxElementBlock
{
    [Display(
        Name = "Branching key",
        Description = "Stable key used by journey rules and DDS state.",
        GroupName = SystemTabNames.Content,
        Order = 100)]
    public virtual string BranchingKey { get; set; }
}
