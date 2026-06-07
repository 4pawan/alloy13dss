using System.ComponentModel.DataAnnotations;
using alloy13dss.Models.Forms;
using EPiServer.Web;

namespace alloy13dss.Models.Blocks;

[SiteContentType(
    DisplayName = "Form tool settings",
    Description = "Reusable settings for a form-based tool.",
    GUID = "D9AAAF1E-CDF5-4644-8B7F-7FDD42ED8EC0",
    GroupName = Globals.GroupNames.Specialized)]
[SiteImageUrl]
public class FormToolSettingsBlock : SiteBlockData
{
    [Display(Name = "Tool key", GroupName = SystemTabNames.Content, Order = 10)]
    public virtual string Key { get; set; }

    [Display(Name = "Name", GroupName = SystemTabNames.Content, Order = 20)]
    public virtual string Name { get; set; }

    [Display(Name = "Description", GroupName = SystemTabNames.Content, Order = 30)]
    [UIHint(UIHint.Textarea)]
    public virtual string Description { get; set; }

    [Display(Name = "Form container block", GroupName = SystemTabNames.Content, Order = 40)]
    [AllowedTypes(typeof(DummyFormContainerBlock))]
    public virtual ContentReference FormContainerBlock { get; set; }
}
