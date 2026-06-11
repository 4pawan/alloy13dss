using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public interface IDummyToolSettingsResolver
{
    SettingsPage Resolve();

    SettingsPage Resolve(DummySitePageData page);

    SettingsPage Resolve(ContentReference contentLink);
}
