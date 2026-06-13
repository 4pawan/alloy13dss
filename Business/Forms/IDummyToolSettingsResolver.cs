using alloy13dss.Models.Pages;

namespace alloy13dss.Business.Forms;

public interface IDummyToolSettingsResolver
{
    IDummySettingsPage Resolve();

    IDummySettingsPage Resolve(DummySitePageData page);

    IDummySettingsPage Resolve(ContentReference contentLink);
}
