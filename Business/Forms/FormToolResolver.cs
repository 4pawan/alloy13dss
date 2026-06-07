using alloy13dss.Models.Forms;
using alloy13dss.Models.Blocks;
using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public class FormToolResolver(IContentLoader contentLoader) : IFormToolResolver
{
    public DummyFormContainerBlock Resolve(SitePageData page)
    {
        if (page == null)
        {
            return null;
        }

        if (!ContentReference.IsNullOrEmpty(page.DirectFormContainerBlock) &&
            contentLoader.TryGet(page.DirectFormContainerBlock, out DummyFormContainerBlock directForm))
        {
            return directForm;
        }

        if (ContentReference.IsNullOrEmpty(page.SettingsPageLink) ||
            string.IsNullOrWhiteSpace(page.FormToolKey) ||
            !contentLoader.TryGet(page.SettingsPageLink, out SettingsPage settingsPage))
        {
            return null;
        }

        var tool = settingsPage.FormTools?
            .Items
            .Select(x => contentLoader.TryGet(x.ContentLink, out FormToolSettingsBlock toolBlock) ? toolBlock : null)
            .FirstOrDefault(x => x != null && string.Equals(x.Key, page.FormToolKey, StringComparison.OrdinalIgnoreCase));

        if (tool == null || ContentReference.IsNullOrEmpty(tool.FormContainerBlock))
        {
            return null;
        }

        return contentLoader.TryGet(tool.FormContainerBlock, out DummyFormContainerBlock configuredForm)
            ? configuredForm
            : null;
    }
}
