using alloy13dss.Models.Pages;
using EPiServer.Web.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace alloy13dss.Business.Forms;

public class DummyToolSettingsResolver(
    IContentLoader contentLoader,
    IPageRouteHelper routeHelper,
    IMemoryCache memoryCache) : IDummyToolSettingsResolver
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public IDummySettingsPage Resolve()
    {
        return routeHelper.Page is IContent page
            ? ResolveFromCachedSettingsLink(page)
            : null;
    }

    public IDummySettingsPage Resolve(DummySitePageData page)
    {
        return page == null ? null : ResolveFromCachedSettingsLink(page);
    }

    public IDummySettingsPage Resolve(ContentReference contentLink)
    {
        if (ContentReference.IsNullOrEmpty(contentLink) ||
            !contentLoader.TryGet(contentLink, out IContent content))
        {
            return null;
        }

        return ResolveFromCachedSettingsLink(content);
    }

    private IDummySettingsPage ResolveFromCachedSettingsLink(IContent content)
    {
        var settingsPageLink = memoryCache.GetOrCreate(
            BuildCacheKey(content),
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
        var rootFolder = FindAncestor<DummyRootFolder>(content);

        return rootFolder != null &&
            !ContentReference.IsNullOrEmpty(rootFolder.SettingsPageLink)
                ? rootFolder.SettingsPageLink
                : ContentReference.EmptyReference;
    }

    private T FindAncestor<T>(IContent content) where T : class, IContent
    {
        var current = content;

        while (current != null)
        {
            if (current is T ancestor)
            {
                return ancestor;
            }

            current = !ContentReference.IsNullOrEmpty(current.ParentLink) &&
                !current.ParentLink.CompareToIgnoreWorkID(ContentReference.RootPage) &&
                contentLoader.TryGet(current.ParentLink, out IContent parent)
                ? parent
                : null;
        }

        return null;
    }

    private bool TryResolveSettingsPage(ContentReference settingsPageLink, out IDummySettingsPage settingsPage)
    {
        settingsPage = null;

        if (ContentReference.IsNullOrEmpty(settingsPageLink) ||
            !contentLoader.TryGet(settingsPageLink, out IContent content) ||
            content is not IDummySettingsPage dummySettingsPage)
        {
            return false;
        }

        settingsPage = dummySettingsPage;
        return true;
    }

    private static string BuildCacheKey(IContent content)
    {
        var contentLink = content.ContentLink;
        var providerName = string.IsNullOrWhiteSpace(contentLink.ProviderName)
            ? "default"
            : contentLink.ProviderName;
        var language = content is ILocalizable localizable
            ? localizable.Language.Name
            : "neutral";

        return $"dummy-tool-settings:{providerName}:{contentLink.ID}:{language}";
    }
}
