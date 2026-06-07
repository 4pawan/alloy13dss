using EPiServer.Forms.Implementation.Elements;

namespace alloy13dss.Models.Forms;

[ContentType(
    DisplayName = "Dummy form container",
    Description = "Renders one Forms element at a time with editor-managed branching.",
    GUID = "45750D5D-8011-43F9-86F5-A1F991FF57A5",
    GroupName = "Forms")]
[SiteImageUrl]
public class DummyFormContainerBlock : FormContainerBlock
{
}
