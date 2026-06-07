using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public interface IToolSettingsResolver
{
    SettingsPage Resolve(DummySitePageData page);

    SettingsPage Resolve(ContentReference contentLink);
}
