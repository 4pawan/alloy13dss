using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public class ToolSettingsResolver(IContentLoader contentLoader) : IToolSettingsResolver
{
    public SettingsPage Resolve(SitePageData page)
    {
        return page == null ? null : ResolveFromContent(page);
    }

    public SettingsPage Resolve(ContentReference contentLink)
    {
        if (ContentReference.IsNullOrEmpty(contentLink) ||
            !contentLoader.TryGet(contentLink, out IContent content))
        {
            return null;
        }

        return ResolveFromContent(content);
    }

    private SettingsPage ResolveFromContent(IContent content)
    {
        var current = content;

        while (current != null)
        {
            if (current is RootFolder rootFolder &&
                TryResolveSettingsPage(rootFolder.SettingsPageLink, out var settingsPage))
            {
                return settingsPage;
            }

            if (ContentReference.IsNullOrEmpty(current.ParentLink) ||
                current.ParentLink.CompareToIgnoreWorkID(ContentReference.RootPage))
            {
                return null;
            }

            current = contentLoader.TryGet(current.ParentLink, out IContent parent)
                ? parent
                : null;
        }

        return null;
    }

    private bool TryResolveSettingsPage(ContentReference settingsPageLink, out SettingsPage settingsPage)
    {
        settingsPage = null;

        return !ContentReference.IsNullOrEmpty(settingsPageLink) &&
            contentLoader.TryGet(settingsPageLink, out settingsPage);
    }
}
