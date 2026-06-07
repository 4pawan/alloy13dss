using System.ComponentModel.DataAnnotations;
using alloy13dss.Models.Forms;

namespace alloy13dss.Models.Pages;

/// <summary>
/// Base class for dummy tool pages that can select or directly host a form tool.
/// </summary>
public abstract class DummySitePageData : SitePageData
{
    [Display(
        Name = "Form tool key",
        Description = "Key of the tool definition on the selected root folder settings page.",
        GroupName = SystemTabNames.Settings,
        Order = 410)]
    public virtual string FormToolKey { get; set; }

    [Display(
        Name = "Direct form container block",
        Description = "Optional page-level override. If set, this block is used instead of the root folder tool key.",
        GroupName = SystemTabNames.Settings,
        Order = 420)]
    [AllowedTypes(typeof(DummyFormContainerBlock))]
    public virtual ContentReference DirectFormContainerBlock { get; set; }
}
