using alloy13dss.Models.Pages;
using Microsoft.Extensions.Caching.Memory;

namespace alloy13dss.Business.Forms;

public class DummyToolSettingsResolver(
    IContentLoader contentLoader,
    IMemoryCache memoryCache) : IDummyToolSettingsResolver
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public SettingsPage Resolve(DummySitePageData page)
    {
        return page == null ? null : ResolveFromCachedSettingsLink(page);
    }

    public SettingsPage Resolve(ContentReference contentLink)
    {
        if (ContentReference.IsNullOrEmpty(contentLink) ||
            !contentLoader.TryGet(contentLink, out IContent content))
        {
            return null;
        }

        return ResolveFromCachedSettingsLink(content);
    }

    private SettingsPage ResolveFromCachedSettingsLink(IContent content)
    {
        var settingsPageLink = memoryCache.GetOrCreate(
            BuildCacheKey(content.ContentLink),
            entry =>
            {
                entry.SlidingExpiration = CacheDuration;
                return ResolveSettingsPageLinkFromContent(content);
            });

        return TryResolveSettingsPage(settingsPageLink, out var settingsPage)
            ? settingsPage
            : null;
    }

    private ContentReference ResolveSettingsPageLinkFromContent(IContent content)
    {
        var current = content;

        while (current != null)
        {
            if (current is DummyRootFolder rootFolder &&
                !ContentReference.IsNullOrEmpty(rootFolder.SettingsPageLink))
            {
                return rootFolder.SettingsPageLink;
            }

            if (ContentReference.IsNullOrEmpty(current.ParentLink) ||
                current.ParentLink.CompareToIgnoreWorkID(ContentReference.RootPage))
            {
                return ContentReference.EmptyReference;
            }

            current = contentLoader.TryGet(current.ParentLink, out IContent parent)
                ? parent
                : null;
        }

        return ContentReference.EmptyReference;
    }

    private bool TryResolveSettingsPage(ContentReference settingsPageLink, out SettingsPage settingsPage)
    {
        settingsPage = null;

        return !ContentReference.IsNullOrEmpty(settingsPageLink) &&
            contentLoader.TryGet(settingsPageLink, out settingsPage);
    }

    private static string BuildCacheKey(ContentReference contentLink)
    {
        var providerName = string.IsNullOrWhiteSpace(contentLink.ProviderName)
            ? "default"
            : contentLink.ProviderName;

        return $"dummy-tool-settings:{providerName}:{contentLink.ID}";
    }
}
